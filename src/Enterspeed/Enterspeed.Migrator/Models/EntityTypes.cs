using System.Collections.Generic;

namespace Enterspeed.Migrator.Models
{
    public class EntityTypes
    {
        public EntityTypes()
        {
            Pages = new List<EntityType>();
            Elements = new List<EntityType>();
            Compositions = new List<EntityType>();
        }

        public List<EntityType> Pages { get; }
        public List<EntityType> Elements { get; }
        public List<EntityType> Compositions { get; }
    }
}