using System.Threading.Tasks;

namespace Umbraco9.Migrator.Builders.Contracts
{
    public interface IUmbracoMigratorService
    {
        Task ImportDocumentTypesAsync();
        Task ImportDataAsync();
    }
}