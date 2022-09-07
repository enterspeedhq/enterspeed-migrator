using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Models
{
    public class PageData
    {
        public PageData()
        {
            Properties = new List<EnterspeedPropertyType>();
            Children = new List<PageData>();
        }

        public MetaSchema MetaSchema { get; set; }
        public List<EnterspeedPropertyType> Properties { get; set; }
        public List<PageData> Children { get; set; }
    }
}