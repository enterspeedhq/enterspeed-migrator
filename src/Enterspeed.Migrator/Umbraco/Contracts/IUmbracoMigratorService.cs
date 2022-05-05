using System.Threading.Tasks;

namespace Enterspeed.Migrator.Contracts
{
    public interface IUmbracoMigratorService
    {
        Task BuildUmbracoDataAsync();
    }
}