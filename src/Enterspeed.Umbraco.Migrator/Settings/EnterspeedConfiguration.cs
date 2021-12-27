using System.Collections.Generic;

namespace Enterspeed.Umbraco.Migrator.Settings
{
    public class EnterspeedConfiguration
    {
        public string ApiKey { get; init; }
        public IDictionary<string, string> NavigationHandles { get; init; }
    }
}
