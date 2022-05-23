using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Models
{
    public class EntityType
    {
        public EntityTypeMeta Meta { get; set; }
        public List<IPropertyType> Properties { get; set; }
    }
}