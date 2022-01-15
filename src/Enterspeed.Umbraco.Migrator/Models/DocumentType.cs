using System.Collections.Generic;

namespace Enterspeed.Umbraco.Migrator.Models
{
    public class DocumentType
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<IPropertyType> Properties { get; set; }
    }
}