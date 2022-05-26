using System.Threading.Tasks;

namespace Umbraco9.Migrator.Umbraco.Contracts
{
    public interface IUmbracoMigratorService
    {
        Task ImportDocumentTypesAsync();
    }
}