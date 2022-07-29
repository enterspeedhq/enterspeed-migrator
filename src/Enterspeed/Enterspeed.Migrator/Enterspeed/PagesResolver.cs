using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
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
        /// <exception cref="NullReferenceException"></exception>
        public EntityTypeMeta GetMetaDataForPage(DeliveryApiResponse deliveryApiResponse)
        {
            var route = deliveryApiResponse.Response.Route;
            if (!deliveryApiResponse.Response.Route.TryGetValue(_configuration.MigrationPageMetaData, out var migrationPageMetaData))
            {
                throw new NullReferenceException($"{_configuration.MigrationPageMetaData} not found on the schema for {JsonSerializer.Serialize(route)}");
            }

            var serialized = JsonSerializer.Serialize(migrationPageMetaData);
            var parsedMetaData = JsonSerializer.Deserialize<EntityTypeMeta>(serialized);
            if (parsedMetaData != null)
            {
                return parsedMetaData;
            }

            throw new NullReferenceException("Something went wrong when trying to return metadata ");
        }

        public List<PageEntityType> Resolve(PageResponse pageResponse)
        {
            var pageEntityTypes = new List<PageEntityType>();
            var page = GetPageData(pageResponse.DeliveryApiResponse?.Response);

            foreach (var deliveryResponse in pageResponse.Children)
            {
                if (deliveryResponse != null)
                {
                    pageEntityTypes.Add(GetPageData(deliveryResponse?.DeliveryApiResponse.Response));
                    if (deliveryResponse.Children.Any())
                    {
                        foreach (var responseChild in deliveryResponse.Children)
                        {
                            var childEntityTypes = Resolve(responseChild);
                            page.Children.AddRange(childEntityTypes);
                        }
                    }
                }
            }

            pageEntityTypes.Add(page);
            return pageEntityTypes;
        }

        public PageEntityType GetPageData(DeliveryResponse deliveryResponse)
        {
            var route = deliveryResponse.Route;
            var routeSerialized = JsonSerializer.SerializeToElement(route);

            var pageEntityType = new PageEntityType();
            MapPageEntityType(pageEntityType, routeSerialized);

            return pageEntityType;
        }

        private void MapPageEntityType(PageEntityType pageEntityType, JsonElement route, IPropertyType parentProperty = null)
        {
            foreach (var jsonProperty in route.EnumerateObject())
            {
                MapPageEntityType(pageEntityType, jsonProperty, parentProperty);
            }
        }

        private void MapPageEntityType(PageEntityType pageEntityType, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            // If is list 
            var listCreated = CreateListProperty(pageEntityType, jsonProperty, parentProperty);
            if (listCreated)
            {
                return;
            }

            // Complex property
            var complexTypeCreated = CreateComplexType(pageEntityType, jsonProperty, parentProperty);
            if (complexTypeCreated)
            {
                return;
            }

            // Simple property
            CreateSimpleProperty(pageEntityType, jsonProperty, parentProperty);
        }

        private bool CreateListProperty(PageEntityType pageEntityType, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            var created = false;
            if (jsonProperty.Value.GetArrayLength() > 0)
            {
                var newParent = _propertyResolver.Resolve(jsonProperty);
                if (parentProperty != null)
                {
                    parentProperty.ChildProperties.Add(newParent);
                }

                var arrayOfElements = jsonProperty.Value.EnumerateArray();
                foreach (var element in arrayOfElements)
                {
                    MapPageEntityType(pageEntityType, element, newParent);
                }

                pageEntityType.Properties.Add(newParent);
                created = true;
            }

            return created;
        }

        private bool CreateComplexType(PageEntityType pageEntityType, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            var created = false;

            // If is a complex type 
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
                    MapPageEntityType(pageEntityType, childProperty, newParent);
                }

                pageEntityType.Properties.Add(newParent);
                created = true;
            }

            return created;
        }

        private void CreateSimpleProperty(PageEntityType pageEntityType, JsonProperty jsonProperty, IPropertyType parentProperty = null)
        {
            var property = _propertyResolver.Resolve(jsonProperty);
            if (parentProperty != null)
            {
                parentProperty.ChildProperties.Add(property);
            }
            else
            {
                pageEntityType.Properties.Add(property);
            }
        }
    }
}