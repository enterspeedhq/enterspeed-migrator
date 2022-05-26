using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Helpers;
using Enterspeed.Migrator.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SourceImporter : ISourceImporter
    {
        private readonly IApiService _apiService;
        private readonly IElementsResolver _elementsResolver;
        private readonly IPagesResolver _pagesResolver;
        private readonly ILogger<SourceImporter> _logger;

        public SourceImporter(IApiService apiService, IPagesResolver pagesResolver, IElementsResolver elementsResolver, ILogger<SourceImporter> logger)
        {
            _apiService = apiService;
            _pagesResolver = pagesResolver;
            _elementsResolver = elementsResolver;
            _logger = logger;
        }

        public async Task<List<PageEntityType>> ImportDataAsync()
        {
            // Get all routes by navigation handle
            var enterspeedResponse = await _apiService.GetNavigationAsync();

            // Get all urls from the handle deliveryApiResponse
            var urls = UrlHelper.GetUrls(enterspeedResponse);

            // Create page responses
            var apiResponses = await GetPageResponses(urls);

            try
            {
                return GetEntityTypes(apiResponses);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when importing data");
                throw;
            }
        }

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

        private List<PageEntityType> GetEntityTypes(List<DeliveryApiResponse> apiResponses)
        {
            var entityTypes = new List<PageEntityType>();
            foreach (var apiResponse in apiResponses)
            {
                if (apiResponse.Response != null)
                {
                    var metaDataForPage = _pagesResolver.GetMetaDataForPage(apiResponse);
                    var page = (new PageEntityType()
                    {
                        Meta = metaDataForPage,
                        Components = new List<EntityType>()
                    });

                    var elementsOnPage = _elementsResolver.GetAllElementsForPage(apiResponse);
                    foreach (var element in elementsOnPage)
                    {
                        page.Components.Add(element);
                    }

                    entityTypes.Add(page);
                }
            }

            return entityTypes;
        }
    }
}
