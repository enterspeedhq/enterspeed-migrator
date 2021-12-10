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

        public ApiService(IEnterspeedDeliveryService enterspeedDeliveryService, EnterspeedConfiguration enterspeedConfiguration)
        {
            _enterspeedDeliveryService = enterspeedDeliveryService;
            _enterspeedConfiguration = enterspeedConfiguration;
        }

        public async Task<DeliveryApiResponse> GetAllByHandles(IEnumerable<string> handles)
        {
            return await _enterspeedDeliveryService.Fetch(_enterspeedConfiguration.ApiKey, (s) =>
            {
                foreach (var handle in handles)
                {
                    s.WithHandle(handle);
                }
            });
        }
    }
}
