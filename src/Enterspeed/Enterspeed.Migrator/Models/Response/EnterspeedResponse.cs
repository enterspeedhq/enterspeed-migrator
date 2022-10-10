using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enterspeed.Migrator.Models.Response
{
    public class Meta
    {
        [JsonPropertyName("Status")]
        public int Status { get; set; }

        [JsonPropertyName("Redirect")]
        public object Redirect { get; set; }
    }

    public class View
    {
        [JsonPropertyName("self")]
        public Self Self { get; set; }

        [JsonPropertyName("children")]
        public List<Child> Children { get; set; }
    }

    public class LinkItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; }
    }

    public class Self
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("view")]
        public LinkItem View { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Child
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("view")]
        public View View { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Navigation
    {
        [JsonPropertyName("self")]
        public Self Self { get; set; }

        [JsonPropertyName("children")]
        public List<Child> Children { get; set; }
    }

    public class Views
    {
        [JsonPropertyName("navigation")]
        public Navigation Navigation { get; set; }
    }

    public class EnterspeedResponse
    {
        [JsonPropertyName("Meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("Route")]
        public object Route { get; set; }

        [JsonPropertyName("Views")]
        public Views Views { get; set; }
    }
}