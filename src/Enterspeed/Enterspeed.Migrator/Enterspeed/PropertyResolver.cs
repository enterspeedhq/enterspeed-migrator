using System.Text.Json;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Enterspeed
{
    public class PropertyResolver : IPropertyResolver
    {
        public IPropertyType Resolve(JsonProperty jsonProperty)
        {
            return new PropertyType()
            {
                Value = jsonProperty.Value,
                Alias = jsonProperty.Name,
                Name = jsonProperty.Name,
                Type = jsonProperty.GetType().Name,
            };
        }
    }
}