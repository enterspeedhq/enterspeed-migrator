using System;
using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IElementsResolver
    {
        /// <summary>
        /// Gets meta data objects for elements
        /// </summary>
        /// <param name="deliveryApiResponse"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        List<EntityTypeMeta> GetMetaDataForElements(DeliveryApiResponse deliveryApiResponse);
    }
}