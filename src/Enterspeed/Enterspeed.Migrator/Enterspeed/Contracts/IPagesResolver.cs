using System;
using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IPagesResolver
    {
        List<PageEntityType> ResolveFromRoot(PageResponse pageResponse);

        /// <summary>
        /// Gets meta data objects for pages
        /// </summary>
        /// <param name="pageResponse"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        EntityTypeMeta GetMetaDataForPage(DeliveryApiResponse pageResponse);
    }
}