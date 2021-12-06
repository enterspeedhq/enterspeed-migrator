using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    internal interface ISchemaImporter
    {
        IEnumerable<Schema> ImportSchemas();
    }
}