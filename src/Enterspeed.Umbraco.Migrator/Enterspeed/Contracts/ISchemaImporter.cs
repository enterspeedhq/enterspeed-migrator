using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    public interface ISchemaImporter
    {
        Task<IEnumerable<Schema>> ImportSchemasAsync(IEnumerable<string> handles);
    }
}