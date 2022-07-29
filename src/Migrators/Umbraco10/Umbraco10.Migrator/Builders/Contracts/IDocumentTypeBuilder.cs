using Enterspeed.Migrator.Models;

namespace Umbraco10.Migrator.Builders.Contracts
{
    public interface IDocumentTypeBuilder
    {
        void BuildPageDocTypes(Schemas schemas);
        void CreateElementTypes(Schemas schemas);
    }
}