using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    public interface ISchemaImporter
    {
        Task<DocumentTypes> ImportSchemasAsync();
    }
}