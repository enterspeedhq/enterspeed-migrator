using System.Threading.Tasks;

namespace Enterspeed.Umbraco.Migrator.Umbraco.Contracts
{
    public interface IUmbracoMigratorService
    {
        Task BuildUmbracoDataAsync();
    }
}