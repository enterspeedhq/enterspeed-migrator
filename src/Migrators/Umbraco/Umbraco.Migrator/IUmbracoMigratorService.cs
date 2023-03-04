using System.Threading.Tasks;

namespace Umbraco.Migrator
{
    public interface IUmbracoMigratorService
    {
        Task ImportDocumentTypesAsync();
        Task ImportDataAsync();
    }
}