using System;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Microsoft.Extensions.Logging;
using Umbraco9.Migrator.Builders.Contracts;

namespace Umbraco9.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly ISchemaImporter _schemaImporter;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;
        private readonly ISourceImporter _sourceImporter;
        private readonly IContentBuilder _contentBuilder;
        private readonly ILogger<UmbracoMigratorService> _logger;

        public UmbracoMigratorService(ISchemaImporter schemaImporter,
            IDocumentTypeBuilder documentTypeBuilder, ISourceImporter sourceImporter, IContentBuilder contentBuilder, ILogger<UmbracoMigratorService> logger)
        {
            _schemaImporter = schemaImporter;
            _documentTypeBuilder = documentTypeBuilder;
            _sourceImporter = sourceImporter;
            _contentBuilder = contentBuilder;
            _logger = logger;
        }

        public async Task ImportDocumentTypesAsync()
        {
            try
            {
                var entityTypes = await _schemaImporter.ImportSchemasAsync();
                _documentTypeBuilder.BuildPageDocTypes(entityTypes);
                _documentTypeBuilder.CreateElementTypes(entityTypes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "something went wrong when building document types");
                throw;
            }

        }

        public async Task ImportDataAsync()
        {
            try
            {
                var data = await _sourceImporter.ImportDataAsync();
                _contentBuilder.BuildContentPages(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "something went wrong when importing data");
                throw;
            }
        }
    }
}