using System.Threading.Tasks;
using Enterspeed.Migrator.Models.Response;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IApiService
    {
        Task<EnterspeedResponse> GetNavigationAsync(string enterspeedHandle = null);

        /// <summary>
        /// Iterates trough all the pages and maps to a delivery api deliveryApiResponse object.
        /// </summary>
        /// <param name="urls"></param>
        /// <returns>List of DeliveryApiResponse</returns>
        Task<PageResponse> GetPageResponsesAsync(EnterspeedResponse enterspeedResponse);
    }
}