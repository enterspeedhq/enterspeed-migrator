using System.Collections.Generic;

namespace Enterspeed.Migrator.ValueTypes
{
    public class PropertyType : IPropertyType
    {
        public PropertyType()
        {
            ChildProperties = new List<IPropertyType>();
        }

        public string Name { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public List<IPropertyType> ChildProperties { get; set; }
    }
}