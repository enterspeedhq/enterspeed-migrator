using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    public interface IApiService
    {
        Task<EnterspeedResponse> GetNavigationAsync();
        Task<DeliveryApiResponse> GetByUrlsAsync(string url);
    }
}