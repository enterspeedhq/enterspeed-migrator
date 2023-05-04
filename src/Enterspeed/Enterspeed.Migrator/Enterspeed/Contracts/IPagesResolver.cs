using System;
using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Models.Response;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IPagesResolver
    {
        List<PageData> ResolveFromRoot(PageResponse pageResponse);
    }
}