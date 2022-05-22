using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Models
{
    public class EntityType
    {
        public string Name { get; set; }    
        public string Alias { get; set; }
        public List<IPropertyType> Properties { get; set; }
    }
}