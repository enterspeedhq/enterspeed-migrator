using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    internal class SchemaImporter : ISchemaImporter
    {
        private readonly IApiConnector _apiConnector;

        public SchemaImporter(IApiConnector apiConnector)
        {
            _apiConnector = apiConnector;
        }

        public IEnumerable<Schema> ImportSchemas()
        {
            _apiConnector
            throw new NotImplementedException();
        }
    }
}
