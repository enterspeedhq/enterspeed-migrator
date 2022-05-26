using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Umbraco9.Migrator.Builders.Contracts;

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

        public async Task ImportDocumentTypesAsync()
        {
            var entityTypes = await _schemaImporter.ImportSchemasAsync();
            _documentTypeBuilder.BuildPageDocTypes(entityTypes);
            _documentTypeBuilder.CreateElementTypes(entityTypes);
        }
    }
}