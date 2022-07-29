using Enterspeed.Migrator.Models;

namespace Umbraco9.Migrator.Builders.Contracts
{
    public interface IDocumentTypeBuilder
    {
        void BuildPageDocTypes(Schemas schemas);
        void CreateComponents(Schemas schemas);
    }
}