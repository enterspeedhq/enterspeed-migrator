using System;
using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.ValueTypes;

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

        List<IPropertyType> GetAllPropertyTypesForPage();
    }
}