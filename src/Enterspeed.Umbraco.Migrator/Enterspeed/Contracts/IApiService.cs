using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    public interface IApiService
    {
        Task<DeliveryApiResponse> GetAllPagesAsync();
        Task<DeliveryApiResponse> GetByUrlsAsync(IEnumerable<string> urls);
    }
}