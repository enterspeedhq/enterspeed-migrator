using Enterspeed.Migrator.Models;

namespace Umbraco10.Migrator.DocumentTypes
{
    public interface IDocumentTypeBuilder
    {
        void BuildPageDocTypes(Schemas schemas);
    }
}