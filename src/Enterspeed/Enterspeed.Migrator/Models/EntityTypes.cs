using System.Collections.Generic;

namespace Enterspeed.Migrator.Models
{
    public class EntityTypes
    {
        public EntityTypes()
        {
            Pages = new List<Component>();
            Components = new List<Component>();
        }

        public List<Component> Pages { get; }
        public List<Component> Components { get; }
    }
}