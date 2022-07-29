using System.Collections.Generic;

namespace Enterspeed.Migrator.ValueTypes
{
    public interface IPropertyType
    {
        string Name { get; set; }
        string Alias { get; set; }
        string Type { get; set; }
        object Value { get; set; }
        public List<IPropertyType> ChildProperties { get; set; }
    }
}