using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SchemaImporter> _logger;
        private readonly EnterspeedConfiguration _configuration;

        public SchemaImporter(IApiService apiService, ILogger<SchemaImporter> logger, IOptions<EnterspeedConfiguration> configuration)
        {
            _apiService = apiService;
            _logger = logger;
            _configuration = configuration?.Value;
        }

        /// <summary>
        /// Imports all schemas as EntityTypes. 
        /// </summary>
        /// <returns></returns>
        public async Task<EntityTypes> ImportSchemasAsync()
        {
            // Get all routes by navigation handle
            var enterspeedResponse = await _apiService.GetNavigationAsync();

            // Get all urls from the handle deliveryApiResponse
            var urls = GetUrls(enterspeedResponse);

            // Create page responses
            var apiResponses = await GetPageResponses(urls);

            try
            {
                return GetEntityTypes(apiResponses);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when migrating schemas");
                throw;
            }
        }

        /// <summary>
        /// Iterates trough all the pages and maps to a delivery api deliveryApiResponse object.
        /// </summary>
        /// <param name="urls"></param>
        /// <returns>List of DeliveryApiResponse</returns>
        private async Task<List<DeliveryApiResponse>> GetPageResponses(List<string> urls)
        {
            var responses = new List<DeliveryApiResponse>();

            // Get all page responses
            foreach (var url in urls)
            {
                var response = await _apiService.GetByUrlsAsync(url);
                responses.Add(response);
            }

            return responses;
        }

        /// <summary>
        /// Iterates trough the api responses and maps data to all properties on entity types
        /// </summary>
        /// <param name="apiResponses"></param>
        /// <returns>List of entity types </returns>
        private EntityTypes GetEntityTypes(List<DeliveryApiResponse> apiResponses)
        {
            var entityTypes = new EntityTypes();
            foreach (var apiResponse in apiResponses)
            {
                if (apiResponse.Response != null)
                {
                    var metaDataForPage = GetMetaDataPropertiesForPage(apiResponse);
                    if (entityTypes.Pages.All(p => p.Meta.SourceEntityAlias != metaDataForPage.SourceEntityAlias))
                    {
                        entityTypes.Pages.Add(new EntityType
                        {
                            Meta = metaDataForPage
                        });
                    }

                    var metaDataForElements = GetMetaDataPropertiesForElements(apiResponse);
                    foreach (var metaDataForElement in metaDataForElements)
                    {
                        if (entityTypes.Elements.All(e => e.Meta.SourceEntityAlias != metaDataForElement.SourceEntityAlias))
                        {
                            entityTypes.Elements.Add(new EntityType()
                            {
                                Meta = metaDataForElement
                            });
                        }
                    }
                }
            }

            return entityTypes;
        }

        /// <summary>
        /// Finds all pages
        /// </summary>
        /// <param name="deliveryApiResponse"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private EntityTypeMeta GetMetaDataPropertiesForPage(DeliveryApiResponse deliveryApiResponse)
        {
            var route = deliveryApiResponse.Response.Route;

            if (!deliveryApiResponse.Response.Route.TryGetValue(_configuration.MigrationPageMetaData, out var migrationPageMetaData))
            {
                throw new NullReferenceException($"{_configuration.MigrationPageMetaData} not found on the schema for {JsonSerializer.Serialize(route)}");
            }

            // Getting views
            if (migrationPageMetaData is not Dictionary<string, object> migrationPageMetaDataDict || !migrationPageMetaDataDict.TryGetValue("view", out var view))
            {
                throw new NullReferenceException($"View property not found in route {JsonSerializer.Serialize(route)}");
            }

            // Getting page meta data property value
            if (view is not Dictionary<string, object> viewDict || !viewDict.TryGetValue("metaData", out var metaData))
            {
                throw new NullReferenceException($"MetaData property not found in route {JsonSerializer.Serialize(route)}");
            }

            // Get page metadata property and map values
            if (metaData is not Dictionary<string, object> metaDataDict ||
                !metaDataDict.TryGetValue("sourceEntityAlias", out var alias) ||
                !metaDataDict.TryGetValue("sourceEntityName", out var name))
            {
                throw new NullReferenceException($"Meta data values could not be mapped {JsonSerializer.Serialize(route)}");
            }

            return new EntityTypeMeta(alias.ToString(), name.ToString());
        }


        private List<EntityTypeMeta> GetMetaDataPropertiesForElements(DeliveryApiResponse deliveryApiResponse)
        {
            var metadataProperties = new List<EntityTypeMeta>();

            var route = deliveryApiResponse.Response.Route;

            // Check that renderings exists
            if (!deliveryApiResponse.Response.Route.TryGetValue("renderings", out var renderings))
            {
                throw new NullReferenceException($"renderings property not found in deliveryApiResponse {JsonSerializer.Serialize(deliveryApiResponse.Response.Route)}");
            }

            // Check that renderings is dictionary and view exists
            if (renderings is not Dictionary<string, object> renderingsDict || !renderingsDict.TryGetValue("view", out var view))
            {
                throw new NullReferenceException($"View property not found in deliveryApiResponse {JsonSerializer.Serialize(route)}");
            }

            // Check that view is dictionary and items exists
            if (view is not Dictionary<string, object> viewDict || !viewDict.TryGetValue("items", out var items))
            {
                throw new NullReferenceException($"Items property not found in deliveryApiResponse {JsonSerializer.Serialize(route)}");
            }

            // Check that view is dictionary and items exists
            if (items is not List<object> itemsList)
            {
                throw new NullReferenceException($"Data property not found in deliveryApiResponse  {JsonSerializer.Serialize(route)}");
            }

            foreach (var item in itemsList)
            {
                if (item is not Dictionary<string, object> itemDict || !itemDict.TryGetValue("data", out var data)) continue;

                if (data is not Dictionary<string, object> dataDictionary || !dataDictionary.TryGetValue("view", out var dataView)) continue;

                if (dataView is not Dictionary<string, object> dataViewDict || !dataViewDict.TryGetValue(_configuration.MigrationComponentMetaData,
                        out var componentMetaData)) continue;

                if (componentMetaData is not Dictionary<string, object> componentMetaDataDict || !componentMetaDataDict.TryGetValue("view", out var metaDataViewValue)) continue;

                if (metaDataViewValue is not Dictionary<string, object> metaDataViewDict) continue;

                metaDataViewDict.TryGetValue("metaData", out var metaData);

                if (metaData is not Dictionary<string, object> metaDataDict) continue;

                var metaDataProperty = new EntityTypeMeta();
                var keys = metaDataDict.Keys;

                foreach (var key in keys)
                {
                    switch (key)
                    {
                        case "sourceEntityAlias":
                            metaDataProperty.SourceEntityAlias = metaDataDict[key].ToString();
                            break;
                        case "sourceEntityName":
                            metaDataProperty.SourceEntityName = metaDataDict[key].ToString();
                            break;
                    }
                }

                metadataProperties.Add(metaDataProperty);
            }

            return metadataProperties;
        }

        /// <summary>
        /// Iterates trough all navigation items for the handle that handles routes
        /// </summary>
        /// <param name="enterspeedResponse"></param>
        /// <returns>Returns a list of urls for the routes</returns>
        private List<string> GetUrls(EnterspeedResponse enterspeedResponse)
        {
            var urls = new List<string>
            {
                enterspeedResponse.Views.Navigation.Self?.View?.Url
            };

            foreach (var child in enterspeedResponse.Views.Navigation.Children)
            {
                urls.Add(child.View?.Self?.View?.Url);
                if (child.View?.Children != null)
                {
                    foreach (var subChild in child.View.Children)
                    {
                        AddUrl(subChild, urls);
                    }
                }
            }

            return urls;
        }

        private void AddUrl(Child child, ICollection<string> urls)
        {
            urls.Add(child.View.Self?.View?.Url);

            if (child.View.Children == null || !child.View.Children.Any()) return;
            foreach (var subChild in child.View.Children)
            {
                AddUrl(subChild, urls);
            }
        }
    }
}