using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enterspeed.Migrator.Models
{
    public class EntityTypeMeta
    {
        [JsonPropertyName("sourceEntityAlias")]
        public string SourceEntityAlias { get; set; }

        [JsonPropertyName("sourceEntityName")]
        public string SourceEntityName { get; set; }

        [JsonPropertyName("contentName")]
        public string ContentName { get; set; }

        [JsonPropertyName("culture")]
        public string Culture { get; set; }

        [JsonPropertyName("createDate")]
        public string CreateDate { get; set; }

        [JsonPropertyName("updateDate")]
        public string UpdateDate { get; set; }

        [JsonPropertyName("parentId")]
        public string ParentId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("contentPage")]
        public List<ContentPath> ContentPath { get; set; }
    }

    public class ContentPath
    {
        public string Id { get; set; }
    }
}