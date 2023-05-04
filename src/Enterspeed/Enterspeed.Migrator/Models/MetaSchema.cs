using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enterspeed.Migrator.Models
{
    public class MetaSchema
    {
        [JsonPropertyName("sourceEntityAlias")]
        public string SourceEntityAlias { get; set; }

        [JsonPropertyName("sourceEntityName")]
        public string SourceEntityName { get; set; }

        [JsonPropertyName("contentName")]
        public string ContentName { get; set; }
    }

    public class ContentPath
    {
        public string Id { get; set; }
    }
}