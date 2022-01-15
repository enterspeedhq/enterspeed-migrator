using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;
using Enterspeed.Umbraco.Migrator.Settings;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public SchemaImporter(IApiService apiService, EnterspeedConfiguration enterspeedConfiguration)
        {
            _apiService = apiService;
            _enterspeedConfiguration = enterspeedConfiguration;
        }

        public async Task<IEnumerable<Schema>> ImportSchemasAsync()
        {
            var pages = await _apiService.GetAllPagesAsync();

            foreach (var handle in _enterspeedConfiguration.NavigationHandles)
            {
                if (pages.Response.Views.TryGetValue(handle.Name, out var nhn))
                {
                    var navigationHandleName = nhn as Dictionary<string, object>;
                    if (navigationHandleName != null && navigationHandleName.TryGetValue(handle.NavigationItem, out var ngi))
                    {
                        var navigationHandleItems = ngi as Dictionary<string, object>;
                        foreach (var navigationHandleItem in navigationHandleItems?.Values
                                     ?.Cast<Dictionary<string, object>>())
                        {
                            if (navigationHandleItem.TryGetValue("view", out var view))
                            {
                                if (view is Dictionary<string, object> castedView && castedView.TryGetValue("children", out var children))
                                {
                                    var castedChildren = children as List<object>;
                                    foreach (var child in castedChildren)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new List<Schema>();
        }

        private IEnumerable<Schema> BuildSchemas(DeliveryApiResponse deliveryApiResponse)
        {
            var views = deliveryApiResponse.Response.Views;
            throw new NotImplementedException();
        }
    }
}