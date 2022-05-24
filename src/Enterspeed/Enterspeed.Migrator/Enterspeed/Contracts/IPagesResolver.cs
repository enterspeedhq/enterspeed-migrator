using System;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IPagesResolver
    {
        /// <summary>
        /// Gets meta data objects for pages
        /// </summary>
        /// <param name="deliveryApiResponse"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        EntityTypeMeta GetMetaDataForPage(DeliveryApiResponse deliveryApiResponse);
    }
}