using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Models
{
    public class PageEntityType
    {
        public PageEntityType()
        {
            Properties = new List<IPropertyType>();
            Children = new List<PageEntityType>();
        }

        public EntityTypeMeta Meta { get; set; }
        public List<IPropertyType> Properties { get; set; }
        public List<EntityType> Components { get; set; }
        public List<PageEntityType> Children { get; set; }
    }
}
