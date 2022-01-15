using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;
using Enterspeed.Umbraco.Migrator.Settings;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public SchemaImporter(IApiService apiService, EnterspeedConfiguration enterspeedConfiguration)
        {
            _apiService = apiService;
            _enterspeedConfiguration = enterspeedConfiguration;
        }

        public async Task<IEnumerable<DocumentTypes>> ImportSchemasAsync()
        {
            var pages = await _apiService.GetAllPagesAsync();
            
            
            return new List<DocumentTypes>();
        }

        private IEnumerable<DocumentTypes> BuildSchemas(DeliveryApiResponse deliveryApiResponse)
        {
            var views = deliveryApiResponse.Response.Views;
            throw new NotImplementedException();
        }
    }
}