using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SourceImporter : ISourceImporter
    {
        private readonly IApiService _apiService;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public SourceImporter(IApiService apiService, IOptions<EnterspeedConfiguration> enterspeedConfiguration)
        {
            _apiService = apiService;
            _enterspeedConfiguration = enterspeedConfiguration?.Value;
        }

        public async Task<List<object>> ImportAllDataSourcesAsync(Dictionary<string, object> views)
        {
            throw new NotImplementedException();
        }
    }
}
