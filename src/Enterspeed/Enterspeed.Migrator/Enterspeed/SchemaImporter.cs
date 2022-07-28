using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;
        private readonly IElementsResolver _elementsResolver;
        private readonly IPagesResolver _pagesResolver;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public SchemaImporter(IApiService apiService,
            IElementsResolver elementsResolver,
            IPagesResolver pagesResolver,
            IOptions<EnterspeedConfiguration> enterspeedConfiguration)
        {
            _apiService = apiService;
            _elementsResolver = elementsResolver;
            _pagesResolver = pagesResolver;
            _enterspeedConfiguration = enterspeedConfiguration?.Value;
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
            var url = string.Empty;

            foreach (var pageUrl in _enterspeedConfiguration.PageUrls)
            {
                url = enterspeedResponse.Views.Navigation.Self.View.Url.Replace(pageUrl, "");
            }

            var pageResponse = new PageResponse
            {
                DeliveryApiResponse = await _apiService.GetByUrlsAsync(url)
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
                var response = await _apiService.GetByUrlsAsync(child?.View?.Self?.View?.Url);
                var pageResponse = new PageResponse
                {
                    DeliveryApiResponse = response
                };

                if (child?.View?.Children != null && child.View.Children.Any())
                {
                    var mappedResponses = await MapResponseAsync(child.View.Children);
                    pageResponse.Children.AddRange(mappedResponses);
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