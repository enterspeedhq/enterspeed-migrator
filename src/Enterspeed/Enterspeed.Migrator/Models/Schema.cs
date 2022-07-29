using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Models
{
    public class Schema
    {
        public Schema()
        {
            Properties = new List<IPropertyType>();
        }

        public MetaSchema MetaSchema { get; set; }
        public List<IPropertyType> Properties { get; set; }
    }
}