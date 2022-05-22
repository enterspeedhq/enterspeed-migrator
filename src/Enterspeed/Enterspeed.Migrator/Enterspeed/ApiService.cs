using System.Text.Json;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
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

        public async Task<DeliveryApiResponse> GetByUrlsAsync(string url)
        {
            return await _enterspeedDeliveryService.Fetch(_enterspeedConfiguration.ApiKey, (s) => { s.WithUrl(url); });
        }
    }
}