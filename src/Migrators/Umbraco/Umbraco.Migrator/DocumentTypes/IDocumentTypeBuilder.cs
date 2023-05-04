using Enterspeed.Migrator.Models;

namespace Umbraco.Migrator.DocumentTypes
{
    public interface IDocumentTypeBuilder
    {
        void BuildDocTypes(Schemas schemas);
    }
}