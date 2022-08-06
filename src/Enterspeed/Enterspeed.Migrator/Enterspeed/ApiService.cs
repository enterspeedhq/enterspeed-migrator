using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models.Response;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    internal class ApiService : IApiService
    {
        private readonly IEnterspeedDeliveryService _enterspeedDeliveryService;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public ApiService(IEnterspeedDeliveryService enterspeedDeliveryService,
            IOptions<EnterspeedConfiguration> enterspeedConfiguration)
        {
            _enterspeedDeliveryService = enterspeedDeliveryService;
            _enterspeedConfiguration = enterspeedConfiguration?.Value;
        }

        public async Task<EnterspeedResponse> GetNavigationAsync()
        {
            var data = await _enterspeedDeliveryService.Fetch(_enterspeedConfiguration.ApiKey,
                (s) => { s.WithHandle(_enterspeedConfiguration.NavigationHandle); });

            var json = JsonSerializer.Serialize(data.Response);
            return JsonSerializer.Deserialize<EnterspeedResponse>(json);
        }

        public async Task<DeliveryApiResponse> GetByUrlAsync(string url)
        {
            return await _enterspeedDeliveryService.Fetch(_enterspeedConfiguration.ApiKey, (s) => { s.WithUrl(url); });
        }

        /// <summary>
        /// Iterates trough all the pages and maps to a delivery api deliveryApiResponse object.
        /// </summary>
        /// <param name="urls"></param>
        /// <returns>List of DeliveryApiResponse</returns>
        public async Task<PageResponse> GetPageResponsesAsync(EnterspeedResponse enterspeedResponse)
        {
            var url = string.Empty;

            foreach (var pageUrl in _enterspeedConfiguration.PageUrls)
            {
                url = enterspeedResponse.Views.Navigation.Self.View.Url.Replace(pageUrl, "");
            }

            var pageResponse = new PageResponse
            {
                DeliveryApiResponse = await GetByUrlAsync(url)
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
                var url = string.Empty;

                foreach (var pageUrl in _enterspeedConfiguration.PageUrls)
                {
                    url = child?.View?.Self?.View?.Url?.Replace(pageUrl, "");
                }

                var response = await GetByUrlAsync(url);
                if (response.Response == null) continue;

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
    }
}