using System.Collections.Generic;
using Enterspeed.Umbraco.Migrator.ValueTypes;

namespace Enterspeed.Umbraco.Migrator.Models
{
    public class DocumentType
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<IPropertyType> Properties { get; set; }
    }
}