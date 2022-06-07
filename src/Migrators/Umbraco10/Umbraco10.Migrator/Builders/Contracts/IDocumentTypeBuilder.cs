using Enterspeed.Migrator.Models;

namespace Umbraco10.Migrator.Builders.Contracts
{
    public interface IDocumentTypeBuilder
    {
        void BuildPageDocTypes(EntityTypes entityTypes);
        void CreateElementTypes(EntityTypes entityTypes);
    }
}