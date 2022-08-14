using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Constants;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Models.Response;
using Enterspeed.Migrator.Settings;
using Enterspeed.Migrator.ValueTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    public class PagesResolver : IPagesResolver
    {
        private readonly EnterspeedConfiguration _configuration;
        private readonly ILogger<PagesResolver> _logger;

        public PagesResolver(
            IOptions<EnterspeedConfiguration> configuration,
            ILogger<PagesResolver> logger)
        {
            _logger = logger;
            _configuration = configuration?.Value;
        }

        /// <summary>
        /// Gets meta data objects for pages
        /// </summary>
        /// <param name="deliveryResponse"></param>ƒ
        /// <returns></returns>
        public MetaSchema GetMetaData(DeliveryResponse deliveryResponse)
        {
            var route = deliveryResponse.Route;
            if (route == null)
            {
                _logger.LogError("Route was not found for");
                return null;
            }

            if (!deliveryResponse.Route.TryGetValue(_configuration.MigrationPageMetaData, out var migrationPageMetaData))
            {
                throw new NullReferenceException($"{_configuration.MigrationPageMetaData} not found on the schema for {JsonSerializer.Serialize(route)}");
            }

            var serialized = JsonSerializer.Serialize(migrationPageMetaData);
            var parsedMetaData = JsonSerializer.Deserialize<MetaSchema>(serialized);
            if (parsedMetaData != null)
            {
                return parsedMetaData;
            }

            throw new NullReferenceException("Something went wrong when trying to return metadata ");
        }

        public List<PageData> ResolveFromRoot(PageResponse pageResponse)
        {
            var pageEntityTypes = new List<PageData>();
            if (pageResponse.DeliveryApiResponse?.Response != null)
            {
                var page = GetPageData(pageResponse.DeliveryApiResponse?.Response);

                foreach (var deliveryResponse in pageResponse.Children)
                {
                    if (deliveryResponse.DeliveryApiResponse.Response != null)
                    {
                        page.Children.AddRange(ResolveFromRoot(deliveryResponse));
                    }
                }

                pageEntityTypes.Add(page);
                return pageEntityTypes;
            }

            return null;
        }

        private PageData GetPageData(DeliveryResponse deliveryResponse)
        {
            var route = deliveryResponse.Route;
            var routeSerialized = JsonSerializer.SerializeToElement(route);

            var pageEntityType = new PageData
            {
                MetaSchema = GetMetaData(deliveryResponse)
            };

            MapData(pageEntityType, routeSerialized);

            return pageEntityType;
        }

        private void MapData(PageData pageData, JsonElement route, EnterspeedPropertyType parentEnterspeedProperty = null)
        {
            if (route.ValueKind != JsonValueKind.Null)
            {
                var routeObject = route.EnumerateObject();
                var alias = routeObject.GetEnumerator().FirstOrDefault(p => p.Name == EnterspeedPropertyConstants.AliasOf.Alias);
                var isComponent = _configuration.ComponentPropertyTypeKeys.Any(p => p == alias.Value.ToString());

                if (parentEnterspeedProperty != null && isComponent)
                {
                    parentEnterspeedProperty.ChildProperties.Add(new EnterspeedPropertyType
                    {
                        Name = EnterspeedPropertyConstants.IsComponentName,
                        Alias = EnterspeedPropertyConstants.IsComponentAlias,
                        Value = true
                    });
                }

                foreach (var jsonProperty in route.EnumerateObject())
                {
                    MapData(pageData, jsonProperty, parentEnterspeedProperty);
                }
            }
        }

        private void MapData(PageData pageData, JsonProperty jsonProperty, EnterspeedPropertyType parentEnterspeedProperty = null)
        {
            switch (jsonProperty.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    CreateObjectType(pageData, jsonProperty, parentEnterspeedProperty);
                    break;
                case JsonValueKind.Array:
                    CreateArrayType(pageData, jsonProperty, parentEnterspeedProperty);
                    break;
                default:
                    CreateSimpleType(pageData, jsonProperty, parentEnterspeedProperty);
                    break;
            }
        }

        private void CreateArrayType(PageData pageData, JsonProperty jsonProperty, EnterspeedPropertyType parentEnterspeedProperty = null)
        {
            if (jsonProperty.Value.ValueKind == JsonValueKind.Array && jsonProperty.Value.GetArrayLength() > 0)
            {
                var currentProperty = new EnterspeedPropertyType(jsonProperty);
                if (parentEnterspeedProperty != null)
                {
                    parentEnterspeedProperty.ChildProperties.Add(currentProperty);
                }

                var arrayOfElements = jsonProperty.Value.EnumerateArray();
                foreach (var element in arrayOfElements)
                {
                    var objectOfElement = element.EnumerateObject();
                    var newArrayItem = new EnterspeedPropertyType()
                    {
                        Name = "arrayObject",
                        Alias = "arrayObject",
                        Type = JsonValueKind.Object,
                        Value = objectOfElement
                    };

                    // Add arrayitem directly to array property
                    currentProperty.ChildProperties.Add(newArrayItem);

                    MapData(pageData, element, newArrayItem);
                }

                // Ensure that we do not add nested properties to the root level of the properties for the page.
                if (parentEnterspeedProperty == null)
                {
                    pageData.Properties.Add(currentProperty);
                }
            }
        }

        private void CreateObjectType(PageData pageData, JsonProperty jsonProperty, EnterspeedPropertyType parentEnterspeedProperty = null)
        {
            // If is a complex type 
            if (jsonProperty.Value.ValueKind == JsonValueKind.Object)
            {
                var elmenent = jsonProperty.Value;
                var currentProperty = new EnterspeedPropertyType(jsonProperty);

                if (parentEnterspeedProperty != null)
                {
                    parentEnterspeedProperty.ChildProperties.Add(currentProperty);
                }

                MapData(pageData, elmenent, currentProperty);

                pageData.Properties.Add(currentProperty);
            }
        }

        private void CreateSimpleType(PageData pageData, JsonProperty jsonProperty, EnterspeedPropertyType parentEnterspeedProperty = null)
        {
            var property = new EnterspeedPropertyType(jsonProperty);
            if (parentEnterspeedProperty != null)
            {
                parentEnterspeedProperty.ChildProperties.Add(property);
            }
            else
            {
                pageData.Properties.Add(property);
            }
        }
    }
}