using System.Collections.Generic;
using Newtonsoft.Json;

//this class is used to mock the correct JSON structure when the object is serialized
namespace Umbraco9.Migrator.DataTypes
{
    public class Blocklist
    {
        public Blocklist()
        {
        }

        public BlockListUdi layout { get; set; }
        public List<Dictionary<string, string>> contentData { get; set; }
        public List<Dictionary<string, string>> settingsData { get; set; }
    }

    //this is a subclass which corresponds to the "Umbraco.BlockList" section in JSON
    public class BlockListUdi
    {
        //we mock the Umbraco.BlockList name with JsonPropertyAttribute to match the requested JSON structure
        [JsonProperty("Umbraco.BlockList")]
        public List<Dictionary<string, string>> contentUdi { get; set; }
        //we do not serialize settingsUdi
        [JsonIgnore]
        public List<Dictionary<string, string>> settingsUdi { get; set; }
        public BlockListUdi(List<Dictionary<string, string>> items, List<Dictionary<string, string>> settings)
        {
            this.contentUdi = items;
            this.settingsUdi = settings;
        }
    }
}
