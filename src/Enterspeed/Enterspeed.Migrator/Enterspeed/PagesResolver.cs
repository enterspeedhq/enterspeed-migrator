using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Models.Response;
using Enterspeed.Migrator.Settings;
using Enterspeed.Migrator.ValueTypes;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    public class PagesResolver : IPagesResolver
    {
        private readonly EnterspeedConfiguration _configuration;
        private readonly IPropertyResolver _propertyResolver;

        public PagesResolver(
            IOptions<EnterspeedConfiguration> configuration,
            IPropertyResolver propertyResolver)
        {
            _propertyResolver = propertyResolver;
            _configuration = configuration?.Value;
        }

        /// <summary>
        /// Gets meta data objects for pages
        /// </summary>
        /// <param name="deliveryApiResponse"></param>
        /// <returns></returns>
        public MetaSchema GetMetaData(DeliveryApiResponse deliveryApiResponse)
        {
            var route = deliveryApiResponse.Response.Route;
            if (!deliveryApiResponse.Response.Route.TryGetValue(_configuration.MigrationPageMetaData, out var migrationPageMetaData))
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
                        var pageEntityType = GetPageData(deliveryResponse.DeliveryApiResponse?.Response);
                        pageEntityTypes.Add(pageEntityType);

                        if (deliveryResponse.Children.Any())
                        {
                            foreach (var responseChild in deliveryResponse.Children)
                            {
                                var childEntityTypes = ResolveFromRoot(responseChild);
                                if (childEntityTypes != null)
                                {
                                    page.Children.AddRange(childEntityTypes);
                                }
                            }
                        }
                    }
                }

                pageEntityTypes.Add(page);
                return pageEntityTypes;
            }

            return null;
        }

        public PageData GetPageData(DeliveryResponse deliveryResponse)
        {
            var route = deliveryResponse.Route;
            var routeSerialized = JsonSerializer.SerializeToElement(route);

            var pageEntityType = new PageData();
            MapPageData(pageEntityType, routeSerialized);

            return pageEntityType;
        }

        private void MapPageData(PageData pageData, JsonElement route, IPropertyType parentProperty = null)
        {
            if (route.ValueKind != JsonValueKind.Null)
            {
                foreach (var jsonProperty in route.EnumerateObject())
                {
                    MapPageData(pageData, jsonProperty, parentProperty);
                }
            }
        }

        private void MapPageData(PageData pageData, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            switch (jsonProperty.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    CreateObjectType(pageData, jsonProperty, parentProperty);
                    break;
                case JsonValueKind.Array:
                    CreateArrayType(pageData, jsonProperty, parentProperty);
                    break;
                default:
                    CreateSimpleType(pageData, jsonProperty, parentProperty);
                    break;
            }
        }

        private void CreateArrayType(PageData pageData, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            if (jsonProperty.Value.ValueKind == JsonValueKind.Array && jsonProperty.Value.GetArrayLength() > 0)
            {
                var newParent = _propertyResolver.Resolve(jsonProperty);
                if (parentProperty != null)
                {
                    parentProperty.ChildProperties.Add(newParent);
                }

                var arrayOfElements = jsonProperty.Value.EnumerateArray();
                foreach (var element in arrayOfElements)
                {
                    MapPageData(pageData, element, newParent);
                }

                pageData.Properties.Add(newParent);
            }
        }

        private void CreateObjectType(PageData pageData, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            // If is a complex type 
            if (jsonProperty.Value.ValueKind == JsonValueKind.Object)
            {
                var listOfProperties = jsonProperty.Value.EnumerateObject();
                if (listOfProperties.Any())
                {
                    var newParent = _propertyResolver.Resolve(jsonProperty);
                    if (parentProperty != null)
                    {
                        parentProperty.ChildProperties.Add(newParent);
                    }

                    foreach (var childProperty in listOfProperties)
                    {
                        MapPageData(pageData, childProperty, newParent);
                    }

                    pageData.Properties.Add(newParent);
                }
            }
        }

        private void CreateSimpleType(PageData pageData, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            var property = _propertyResolver.Resolve(jsonProperty);
            if (parentProperty != null)
            {
                parentProperty.ChildProperties.Add(property);
            }
            else
            {
                pageData.Properties.Add(property);
            }
        }
    }
}