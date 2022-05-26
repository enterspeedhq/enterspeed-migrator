using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Helpers;
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

            // Create page responses
            var apiResponses = await GetPageResponses(enterspeedResponse);

            return GetEntityTypes(apiResponses);

        }

        /// <summary>
        /// Iterates trough all the pages and maps to a delivery api deliveryApiResponse object.
        /// </summary>
        /// <param name="urls"></param>
        /// <returns>List of DeliveryApiResponse</returns>
        private async Task<PageResponse> GetPageResponses(EnterspeedResponse enterspeedResponse)
        {
            var pageResponse = new PageResponse
            {
                DeliveryApiResponse = await _apiService.GetByUrlsAsync(enterspeedResponse.Views.Navigation.Self.View.Url)
            };

            var children = await MapResponseAsync(enterspeedResponse.Views.Navigation?.Children);
            pageResponse.Children.AddRange(children);

            return pageResponse;
        }

        private async Task<List<PageResponse>> MapResponseAsync(List<Child> children)
        {
            var pageResponses = new List<PageResponse>();
            foreach (var child in children)
            {
                var response = await _apiService.GetByUrlsAsync(child.View.Self.View.Url);
                var pageResponse = new PageResponse
                {
                    DeliveryApiResponse = response
                };

                if (child.View.Children != null && child.View.Children.Any())
                {
                    pageResponse.Children.AddRange(await MapResponseAsync(child.View.Children));
                }

                pageResponses.Add(pageResponse);

            }

            return pageResponses;
        }


        /// <summary>
        /// Iterates trough the api responses and maps data to all properties on entity types
        /// </summary>
        /// <param name="pageResponse"></param>
        /// <returns>List of entity types </returns>
        private EntityTypes GetEntityTypes(PageResponse pageResponse)
        {
            var entityTypes = new EntityTypes();
            GetEntityType(pageResponse, entityTypes);

            foreach (var apiResponse in pageResponse.Children)
            {
                GetEntityType(apiResponse, entityTypes);
            }

            return entityTypes;
        }

        private void GetEntityType(PageResponse apiResponse, EntityTypes entityTypes)
        {
            if (apiResponse.DeliveryApiResponse != null)
            {
                var metaDataForPage = _pagesResolver.GetMetaDataForPage(apiResponse.DeliveryApiResponse);
                if (entityTypes.Pages.All(p => p.Meta.SourceEntityAlias != metaDataForPage.SourceEntityAlias))
                {
                    entityTypes.Pages.Add(new EntityType
                    {
                        Meta = metaDataForPage
                    });
                }

                var elementsOnPage = _elementsResolver.GetAllElementsForPage(apiResponse.DeliveryApiResponse);
                foreach (var element in elementsOnPage)
                {
                    if (element != null && element.Meta != null)
                    {
                        var existingElement = entityTypes.Components.FirstOrDefault(p =>
                            p?.Meta?.SourceEntityAlias == element.Meta?.SourceEntityAlias);
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
    }
}