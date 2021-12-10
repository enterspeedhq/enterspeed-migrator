using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    public interface IApiService
    {
        Task<DeliveryApiResponse> GetAllByHandles(IEnumerable<string> handles);
    }
}