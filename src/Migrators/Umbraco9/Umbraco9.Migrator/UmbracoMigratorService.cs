using System;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Microsoft.Extensions.Logging;
using Umbraco9.Migrator.Builders.Contracts;

namespace Umbraco9.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;
        private readonly IContentBuilder _contentBuilder;
        private readonly ILogger<UmbracoMigratorService> _logger;

        public UmbracoMigratorService(ISchemaBuilder schemaBuilder,
            IDocumentTypeBuilder documentTypeBuilder,IContentBuilder contentBuilder,
            ILogger<UmbracoMigratorService> logger)
        {
            _schemaBuilder = schemaBuilder;
            _documentTypeBuilder = documentTypeBuilder;
            _contentBuilder = contentBuilder;
            _logger = logger;
        }

        public async Task ImportDocumentTypesAsync()
        {
            try
            {
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, "something went wrong when importing data");
                throw;
            }
        }
    }
}