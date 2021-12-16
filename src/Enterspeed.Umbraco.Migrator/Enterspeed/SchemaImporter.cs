using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;

        public SchemaImporter(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<Schema>> ImportSchemasAsync(IEnumerable<string> handles)
        {
            var response = await _apiService.GetAllByHandles(handles);
            return BuildSchemas(response);
        }

        private IEnumerable<Schema> BuildSchemas(DeliveryApiResponse deliveryApiResponse)
        {
            var views = deliveryApiResponse.Response.Views;

            throw new NotImplementedException();
        }
    }
}
