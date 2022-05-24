using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IElementsResolver
    {
        List<EntityType> GetAllElementsForPage(DeliveryApiResponse deliveryApiResponse);
    }
}