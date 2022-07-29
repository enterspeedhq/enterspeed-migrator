using System;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Microsoft.Extensions.Logging;
using Umbraco10.Migrator.Builders.Contracts;

namespace Umbraco10.Migrator
{
    public class UmbracoMigratorService : IUmbracoMigratorService
    {
        private readonly IPagesResolver _pagesResolver;
        private readonly ISourceImporter _sourceImporter;
        private readonly IContentBuilder _contentBuilder;
        private readonly IApiService _apiService;
        private readonly ILogger<UmbracoMigratorService> _logger;

        public UmbracoMigratorService(
            ISourceImporter sourceImporter,
            IContentBuilder contentBuilder,
            ILogger<UmbracoMigratorService> logger,
            IPagesResolver pagesResolver,
            IApiService apiService)
        {
            _sourceImporter = sourceImporter;
            _contentBuilder = contentBuilder;
            _logger = logger;
            _pagesResolver = pagesResolver;
            _apiService = apiService;
        }

        public async Task ImportDocumentTypesAsync()
        {
            try
            {
                var navigation = await _apiService.GetNavigationAsync();
                
                // Page response is root level node/item
                var rootLevelResponse = await _apiService.GetPageResponsesAsync(navigation);
                var pages = _pagesResolver.Resolve(rootLevelResponse);
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