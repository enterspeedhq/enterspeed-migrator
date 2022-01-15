using System.Collections.Generic;

namespace Enterspeed.Umbraco.Migrator.Models
{
    public class DocumentTypes
    {
        public DocumentTypes()
        {
            Pages = new List<DocumentType>();
            Elements = new List<DocumentType>();
            Compositions = new List<DocumentType>();
        }

        public List<DocumentType> Pages { get; }
        public List<DocumentType> Elements { get; }
        public List<DocumentType> Compositions { get; }
    }
}