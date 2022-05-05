using System.Threading.Tasks;
using Enterspeed.Migrator.Contracts;
using Enterspeed.Migrator.Enterspeed.Contracts;

namespace Umbraco9.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly ISchemaImporter _schemaImporter;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;

        public UmbracoMigratorService(ISchemaImporter schemaImporter,
            IDocumentTypeBuilder documentTypeBuilder)
        {
            _schemaImporter = schemaImporter;
            _documentTypeBuilder = documentTypeBuilder;
        }

        public async Task BuildUmbracoDataAsync()
        {
            var schemas = await _schemaImporter.ImportSchemasAsync();
            // var docTypes = await _documentTypeBuilder.BuildDoctypesAsync(schemas.ToList());
        }
    }
}