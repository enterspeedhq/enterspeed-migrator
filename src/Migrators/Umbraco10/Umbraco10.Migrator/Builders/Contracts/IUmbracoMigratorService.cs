using System.Threading.Tasks;

namespace Umbraco10.Migrator.Builders.Contracts
{
    public interface IUmbracoMigratorService
    {
        Task ImportDocumentTypesAsync();
        Task ImportDataAsync();
    }
}