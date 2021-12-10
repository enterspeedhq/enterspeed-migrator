using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    internal class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;

        public SchemaImporter(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<Schema>> ImportSchemas(IEnumerable<string> handles)
        {
            var data = await _apiService.GetAllByHandles(handles);
            BuildSchemas();
            throw new NotImplementedException();
        }

        private IEnumerable<Schema> BuildSchemas()
        {
            throw new NotImplementedException();
        }
    }
}
