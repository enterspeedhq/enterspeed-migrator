using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
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

            // Get all urls from the handle response
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
        /// Iterates trough all the pages and maps to a delivery api response object.
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
                    var baseProperties = GetMetaDataProperties(apiResponse);
                    if (entityTypes.Pages.All(p => p.Meta.Alias != baseProperties.Alias))
                    {
                        entityTypes.Pages.Add(new EntityType
                        {
                            Meta = new EntityTypeMeta()
                            {
                                Alias = baseProperties.Alias,
                                Name = baseProperties.Name
                            }
                        });
                    }
                }
            }

            return entityTypes;
        }

        /// <summary>
        /// Finds all base properties on pages and maps these to 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private EntityTypeMeta GetMetaDataProperties(DeliveryApiResponse response)
        {
            if (!response.Response.Route.TryGetValue(_configuration.MigrationMetaDataKey, out var pageType))
            {
                throw new NullReferenceException($"{_configuration.MigrationMetaDataKey} not found on the schema for {JsonSerializer.Serialize(response.Response.Route)}");
            }

            // Getting views
            if (pageType is not Dictionary<string, object> pageTypeDict || !pageTypeDict.TryGetValue("view", out var view))
            {
                throw new NullReferenceException($"Page types view property not found on schema {JsonSerializer.Serialize(pageType)}");
            }

            // Getting page meta data property value
            if (view is not Dictionary<string, object> viewDict || !viewDict.TryGetValue("metaData", out var @type))
            {
                throw new NullReferenceException($"Page types value property not found on schema {JsonSerializer.Serialize(pageType)}");
            }

            // Get page metadata property and map values
            if (@type is not Dictionary<string, object> typeDict ||
                !typeDict.TryGetValue("alias", out var alias) ||
                !typeDict.TryGetValue("name", out var name))
            {
                throw new NullReferenceException($"Page types alias or name property not found on schema {JsonSerializer.Serialize(pageType)}");
            }

            return new EntityTypeMeta
            {
                Alias = alias.ToString(),
                Name = name.ToString()
            };
        }

        /// <summary>
        /// Iterates trough all navigation items for the handle that handles routes
        /// </summary>
        /// <param name="enterspeedResponse"></param>
        /// <returns>Returns a list of urls for the routes</returns>
        private List<string> GetUrls(EnterspeedResponse enterspeedResponse)
        {
            var urls = new List<string>();
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