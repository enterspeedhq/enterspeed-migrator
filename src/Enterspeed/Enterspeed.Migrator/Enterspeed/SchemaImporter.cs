using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SchemaImporter> _logger;
        private readonly IElementsResolver _elementsResolver;
        private readonly IPagesResolver _pagesResolver;

        public SchemaImporter(IApiService apiService, ILogger<SchemaImporter> logger, IElementsResolver elementsResolver, IPagesResolver pagesResolver)
        {
            _apiService = apiService;
            _logger = logger;
            _elementsResolver = elementsResolver;
            _pagesResolver = pagesResolver;
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
                    var metaDataForPage = _pagesResolver.GetMetaDataForPage(apiResponse);
                    if (entityTypes.Pages.All(p => p.Meta.SourceEntityAlias != metaDataForPage.SourceEntityAlias))
                    {
                        entityTypes.Pages.Add(new EntityType
                        {
                            Meta = metaDataForPage
                        });
                    }

                    var elementsOnPage = _elementsResolver.GetAllElementsForPage(apiResponse);
                    foreach (var element in elementsOnPage)
                    {
                        if (element != null && element.Meta != null)
                        {
                            var existingElement = entityTypes.Components.FirstOrDefault(p => p?.Meta?.SourceEntityAlias == element.Meta?.SourceEntityAlias);
                            if (existingElement != null && element.Properties != null)
                            {
                                foreach (var property in element.Properties)
                                {
                                    if (existingElement.Properties.All(p => p.Alias != property.Alias))
                                    {
                                        existingElement.Properties.Add(property);
                                    }
                                }
                            }
                            else
                            {
                                entityTypes.Components.Add(element);
                            }
                        }
                    }
                }
            }

            return entityTypes;
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