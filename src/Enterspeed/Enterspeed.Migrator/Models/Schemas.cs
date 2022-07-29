using System.Collections.Generic;

namespace Enterspeed.Migrator.Models
{
    public class Schemas
    {
        public Schemas()
        {
            Pages = new List<Schema>();
            Components = new List<Schema>();
        }

        public List<Schema> Pages { get; }
        public List<Schema> Components { get; }
    }
}