using Enterspeed.Migrator.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Enterspeed.Migrator.ValueTypes
{
    public class PropertyType
    {
        public PropertyType(JsonProperty jsonProperty)
        {
            Value = jsonProperty.Value;
            Alias = jsonProperty.Name;
            Name = jsonProperty.Name;
            Type = jsonProperty.Value.ValueKind.ToString();
            ChildProperties = new List<PropertyType>();
        }

        public PropertyType()
        {
        }

        public string Name { get; set; }
        public string Alias { get; set; }
        public string @Type { get; set; }
        public object Value { get; set; }
        public List<PropertyType> ChildProperties { get; set; }

        public bool IsComponent(EnterspeedConfiguration enterspeedConfiguration)
        {
            var childPropertyAlias = ChildProperties.Select(p => p.Alias);
            foreach (var property in childPropertyAlias)
            {
                return enterspeedConfiguration.ComponentPropertyTypeKeys.Any(c => c.Equals(property));
            }

            return false;
        }
    }
}