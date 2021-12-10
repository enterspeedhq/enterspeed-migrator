using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    internal interface ISchemaImporter
    {
        Task<IEnumerable<Schema>> ImportSchemas(IEnumerable<string> handles);
    }
}