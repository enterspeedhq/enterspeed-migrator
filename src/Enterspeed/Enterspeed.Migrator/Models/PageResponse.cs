using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Migrator.Models
{
    public class PageResponse
    {
        public PageResponse()
        {
            Children = new List<PageResponse>();
        }

        public DeliveryApiResponse DeliveryApiResponse { get; set; }
        public List<PageResponse> Children { get; set; }
    }
}
