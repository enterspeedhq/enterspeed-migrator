﻿using Enterspeed.Umbraco.Migrator;
using Enterspeed.Umbraco.Migrator.Enterspeed;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Settings;
using Enterspeed.Umbraco.Migrator.Umbraco.Contracts;

namespace Umbraco91.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly ISchemaImporter _schemaImporter;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;

        public UmbracoMigratorService(SchemaImporter schemaImporter,
            IDocumentTypeBuilder documentTypeBuilder)
        {
            _schemaImporter = schemaImporter;
            _documentTypeBuilder = documentTypeBuilder;
        }

        public async Task BuildUmbracoDataAsync()
        {
            var schemas = await _schemaImporter.ImportSchemasAsync();
            var docTypes = await _documentTypeBuilder.BuildDoctypesAsync(schemas.ToList());
        }
    }
}