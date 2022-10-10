using System;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Microsoft.Extensions.Logging;
using Umbraco10.Migrator.Content;
using Umbraco10.Migrator.DocumentTypes;

namespace Umbraco10.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly IPagesResolver _pagesResolver;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;
        private readonly IApiService _apiService;
        private readonly ILogger<UmbracoMigratorService> _logger;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IContentBuilder _contentBuilder;

        public UmbracoMigratorService(
            ILogger<UmbracoMigratorService> logger,
            IPagesResolver pagesResolver,
            IApiService apiService,
            ISchemaBuilder schemaBuilder,
            IDocumentTypeBuilder documentTypeBuilder,
            IContentBuilder contentBuilder)
        {
            _logger = logger;
            _pagesResolver = pagesResolver;
            _apiService = apiService;
            _schemaBuilder = schemaBuilder;
            _documentTypeBuilder = documentTypeBuilder;
            _contentBuilder = contentBuilder;
        }

        public async Task ImportDocumentTypesAsync()
        {
            try
            {
                // Enterspeed responses
                var navigation = await _apiService.GetNavigationAsync();
                var rootLevelResponse = await _apiService.GetPageResponsesAsync(navigation);

                // All pages with all data
                var pages = _pagesResolver.ResolveFromRoot(rootLevelResponse).ToList();

                // Mapped out data structures in schemas based on the pages
                var pageSchemas = _schemaBuilder.BuildPageSchemas(pages);

                // Build document types based on schemas
                _documentTypeBuilder.BuildDocTypes(pageSchemas);
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
                // Enterspeed responses
                var navigation = await _apiService.GetNavigationAsync();
                var rootLevelResponse = await _apiService.GetPageResponsesAsync(navigation);

                // All pages with all data
                var pages = _pagesResolver.ResolveFromRoot(rootLevelResponse).Where(p => p.MetaSchema != null).ToList();

                // Build content based on pages
                _contentBuilder.BuildContentPages(pages);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "something went wrong when seeding data");
                throw;
            }
        }
    }
}