using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Settings;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    internal class ApiService : IApiService
    {
        private readonly IEnterspeedDeliveryService _enterspeedDeliveryService;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public ApiService(IEnterspeedDeliveryService enterspeedDeliveryService,
            EnterspeedConfiguration enterspeedConfiguration)
        {
            _enterspeedDeliveryService = enterspeedDeliveryService;
            _enterspeedConfiguration = enterspeedConfiguration;
        }

        public async Task<DeliveryApiResponse> GetAllPagesAsync()
        {
            return await _enterspeedDeliveryService.Fetch(_enterspeedConfiguration.ApiKey, (s) =>
            {
                foreach (var handle in _enterspeedConfiguration.NavigationHandles)
                {
                    s.WithHandle(handle.Key);
                }
            });
        }

        public async Task<DeliveryApiResponse> GetByUrlsAsync(IEnumerable<string> urls)
        {
            return await _enterspeedDeliveryService.Fetch(_enterspeedConfiguration.ApiKey, (s) =>
            {
                foreach (var url in urls)
                {
                    s.WithUrl(url);
                }
            });
        }
    }
}