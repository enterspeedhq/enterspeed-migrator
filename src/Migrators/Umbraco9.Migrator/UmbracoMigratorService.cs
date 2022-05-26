using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Umbraco9.Migrator.Builders.Contracts;

namespace Umbraco9.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly ISchemaImporter _schemaImporter;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;
        private readonly ISourceImporter _sourceImporter;
        private readonly IContentBuilder _contentBuilder;

        public UmbracoMigratorService(ISchemaImporter schemaImporter,
            IDocumentTypeBuilder documentTypeBuilder, ISourceImporter sourceImporter, IContentBuilder contentBuilder)
        {
            _schemaImporter = schemaImporter;
            _documentTypeBuilder = documentTypeBuilder;
            _sourceImporter = sourceImporter;
            _contentBuilder = contentBuilder;
        }

        public async Task ImportDocumentTypesAsync()
        {
            var entityTypes = await _schemaImporter.ImportSchemasAsync();
            _documentTypeBuilder.BuildPageDocTypes(entityTypes);
            _documentTypeBuilder.CreateElementTypes(entityTypes);
        }

        public async Task ImportDataAsync()
        {
            var data = await _sourceImporter.ImportDataAsync();
            _contentBuilder.BuildContentPages(data);
        }
    }
}