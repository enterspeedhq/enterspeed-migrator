using System.Collections.Generic;

namespace Enterspeed.Migrator.Models
{
    public class EntityTypes
    {
        public EntityTypes()
        {
            Pages = new List<EntityType>();
            Components = new List<EntityType>();
        }

        public List<EntityType> Pages { get; }
        public List<EntityType> Components { get; }
    }
}