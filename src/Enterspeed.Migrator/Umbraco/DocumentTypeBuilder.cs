using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Enterspeed.Migrator.Umbraco.Contracts;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Migrator.Umbraco
{
    public class DocumentTypeBuilder : IDocumentTypeBuilder
    {
        private readonly ISchemaImporter _schemaImporter;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;
        private readonly ILogger<DocumentTypeBuilder> _logger;

        public DocumentTypeBuilder(ISchemaImporter schemaImporter,
            ILogger<DocumentTypeBuilder> logger, EnterspeedConfiguration enterspeedConfiguration)
        {
            _schemaImporter = schemaImporter;
            _enterspeedConfiguration = enterspeedConfiguration;
            _logger = logger;
        }

        public async Task<IEnumerable<UmbracoDoctype>> BuildDoctypesAsync(List<DocumentTypes> schemas)
        {
            try
            {
                if (schemas != null && schemas.Any())
                    return schemas.Select(s => new UmbracoDoctype(s));

                _logger.LogWarning("No schemas found in import");
                return new List<UmbracoDoctype>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when building schemas");
                throw;
            }
        }
    }
}
