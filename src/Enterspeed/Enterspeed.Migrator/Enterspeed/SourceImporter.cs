using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SourceImporter : ISourceImporter
    {
        private readonly IApiService _apiService;
        private readonly IPagesResolver _pagesResolver;
        private readonly ILogger<SourceImporter> _logger;

        public SourceImporter(IApiService apiService, IPagesResolver pagesResolver, ILogger<SourceImporter> logger)
        {
            _apiService = apiService;
            _pagesResolver = pagesResolver;
            _logger = logger;
        }

        public async Task<List<PageEntityType>> ImportDataAsync()
        {
            // Get all routes by navigation handle
            var enterspeedResponse = await _apiService.GetNavigationAsync();

            // Create page responses
            var apiResponses = await GetPageResponses(enterspeedResponse);

            return GetEntityTypes(apiResponses);
        }

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

        private List<PageEntityType> GetEntityTypes(PageResponse pageResponse)
        {
            return MapPages(pageResponse);
        }


        private List<PageEntityType> MapPages(PageResponse pageResponse)
        {
            var pageEntityTypes = new List<PageEntityType>();
            var pageEntityType = MapPageEntityType(pageResponse);

            foreach (var response in pageResponse.Children)
            {
                pageEntityType.Children.AddRange(MapPages(response));
            }

            pageEntityTypes.Add(pageEntityType);
            return pageEntityTypes;
        }


        private PageEntityType MapPageEntityType(PageResponse pageResponse)
        {
            var metaDataForPage = _pagesResolver.GetMetaDataForPage(pageResponse.DeliveryApiResponse);
            var page = (new PageEntityType()
            {
                Meta = metaDataForPage,
                Components = new List<Component>()
            });

            // var elementsOnPage = _elementsResolver.GetAllDataForPage(pageResponse.DeliveryApiResponse);
            // foreach (var element in elementsOnPage)
            // {
            //     page.Components.Add(element);
            // }

            return page;
        }
    }
}