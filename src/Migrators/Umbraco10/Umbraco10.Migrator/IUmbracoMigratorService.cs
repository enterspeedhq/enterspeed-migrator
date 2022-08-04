using System.Threading.Tasks;

namespace Umbraco10.Migrator
{
    public interface IUmbracoMigratorService
    {
        Task ImportDocumentTypesAsync();
        Task ImportDataAsync();
    }
}