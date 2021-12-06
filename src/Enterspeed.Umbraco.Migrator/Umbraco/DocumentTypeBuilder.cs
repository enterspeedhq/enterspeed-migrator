using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;
using Enterspeed.Umbraco.Migrator.Umbraco.Contracts;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Umbraco.Migrator.Umbraco
{
    public class DocumentTypeBuilder : IDocumentTypeBuilder
    {
        private readonly ISchemaImporter _schemaImporter;
        private readonly ILogger<DocumentTypeBuilder> _logger;

        internal DocumentTypeBuilder(ISchemaImporter schemaImporter,
            ILogger<DocumentTypeBuilder> logger)
        {
            _schemaImporter = schemaImporter;
            _logger = logger;
        }

        public IEnumerable<UmbracoDoctype> BuildDoctypes()
        {
            try
            {
                var schemas = _schemaImporter.ImportSchemas();
                if (schemas != null && schemas.Any())
                    return schemas.Select(s => new UmbracoDoctype(s));

                _logger.LogWarning("No schemas found in import");
                return new List<UmbracoDoctype>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when building schemas")kd;
                throw;
            }
        }
    }
}
