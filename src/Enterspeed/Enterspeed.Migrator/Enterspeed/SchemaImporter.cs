using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        public Task<EntityTypes> ImportSchemasAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}