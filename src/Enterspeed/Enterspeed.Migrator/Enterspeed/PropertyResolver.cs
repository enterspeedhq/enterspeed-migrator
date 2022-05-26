using System.Collections.Generic;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Enterspeed
{
    public class PropertyResolver : IPropertyResolver
    {
        public IPropertyType Resolve(string key, Dictionary<string, object> view)
        {
            if (!key.Contains("meta"))
            {
                var metaKey = key + "_meta";
                var property = view[key];
                if (view.ContainsKey(metaKey))
                {
                    var propertyMeta = view[metaKey] as Dictionary<string, object>;
                    return new PropertyType()
                    {
                        Value = property?.ToString(),
                        Alias = key,
                        Name = key,
                        Type = propertyMeta?["dataType"].ToString(),
                        Source = propertyMeta?["source"].ToString(),
                    };
                }
            }

            return null;
        }
    }
}
