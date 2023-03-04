using System;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using Umbraco.Migrator.Content;
using Umbraco.Migrator.DocumentTypes;

namespace Umbraco.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly IPagesResolver _pagesResolver;
        private readonly IDocumentTypeBuilder _documentTypeBuilder;
        private readonly IApiService _apiService;
        private readonly ILogger<UmbracoMigratorService> _logger;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IContentBuilder _contentBuilder;
        private readonly IContentService _contentService;

        public UmbracoMigratorService(
            ILogger<UmbracoMigratorService> logger,
            IPagesResolver pagesResolver,
            IApiService apiService,
            ISchemaBuilder schemaBuilder,
            IDocumentTypeBuilder documentTypeBuilder,
            IContentBuilder contentBuilder,
            IContentService contentService)
        {
            _logger = logger;
            _pagesResolver = pagesResolver;
            _apiService = apiService;
            _schemaBuilder = schemaBuilder;
            _documentTypeBuilder = documentTypeBuilder;
            _contentBuilder = contentBuilder;
            _contentService = contentService;
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

        public async Task ImportDataAsync(string enterspeedHandle, int? parentId)
        {
            try
            {
                // Enterspeed responses
                var navigation = await _apiService.GetNavigationAsync(enterspeedHandle);
                var rootLevelResponse = await _apiService.GetPageResponsesAsync(navigation);

                // All pages with all data
                var pages = _pagesResolver.ResolveFromRoot(rootLevelResponse).Where(p => p.MetaSchema != null).ToList();

                if (parentId.HasValue)
                {
                    var sectionNode = _contentService.GetById(parentId.Value);
                    // We want to leave out root page, since we are only interested in all children.
                    _contentBuilder.BuildContentPagesInSection(pages.FirstOrDefault()?.Children.Where(p => p.MetaSchema != null).ToList(), sectionNode);
                }
                else
                {
                    // Build content based on pages
                    _contentBuilder.BuildContentPages(pages);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "something went wrong when seeding data");
                throw;
            }
        }
    }
}