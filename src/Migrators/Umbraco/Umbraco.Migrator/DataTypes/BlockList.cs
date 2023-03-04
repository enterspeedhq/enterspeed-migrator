using System.Collections.Generic;
using Newtonsoft.Json;

namespace Umbraco.Migrator.DataTypes
{
    public class Blocklist
    {
        public BlockListUdi layout { get; set; }
        public List<Dictionary<string, object>> contentData { get; set; }
        public List<Dictionary<string, string>> settingsData { get; set; }
    }

    public class BlockListUdi
    {
        [JsonProperty("Umbraco.BlockList")]
        public List<Dictionary<string, string>> contentUdi { get; set; }

        public BlockListUdi(List<Dictionary<string, string>> items)
        {
            contentUdi = items;
        }
    }
}
