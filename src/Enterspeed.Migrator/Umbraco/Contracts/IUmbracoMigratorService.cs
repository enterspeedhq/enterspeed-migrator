using System.Threading.Tasks;

namespace Enterspeed.Migrator.Umbraco.Contracts
{
    public interface IUmbracoMigratorService
    {
        Task BuildUmbracoDataAsync();
    }
}