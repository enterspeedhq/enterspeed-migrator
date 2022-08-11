using Enterspeed.Migrator.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Enterspeed.Migrator.ValueTypes
{
    public class EnterspeedPropertyType
    {
        public EnterspeedPropertyType(JsonProperty jsonProperty)
        {
            Value = jsonProperty.Value;
            Alias = jsonProperty.Name;
            Name = jsonProperty.Name;
            Type = jsonProperty.Value.ValueKind;
            ChildProperties = new List<EnterspeedPropertyType>();
        }

        public EnterspeedPropertyType()
        {
            ChildProperties = new List<EnterspeedPropertyType>();
        }

        public string Name { get; set; }
        public string Alias { get; set; }
        public JsonValueKind @Type { get; set; }
        public object Value { get; set; }
        public List<EnterspeedPropertyType> ChildProperties { get; set; }

        public bool IsComponent()
        {
            return ChildProperties.Any(p => p.Alias == EnterspeedPropertyConstants.IsComponentAlias);
        }
    }
}