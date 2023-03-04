using System.Threading.Tasks;

namespace Umbraco.Migrator
{
    public interface IUmbracoMigratorService
    {
        Task ImportDocumentTypesAsync();
        Task ImportDataAsync(string enterspeedHandle = null, int? parentId = null);
    }
}