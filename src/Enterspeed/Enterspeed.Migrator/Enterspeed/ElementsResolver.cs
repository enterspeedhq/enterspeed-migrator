using System;
using System.Collections.Generic;
using System.Text.Json;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Options;

namespace Enterspeed.Migrator.Enterspeed
{
    public class ElementsResolver : IElementsResolver
    {
        private readonly EnterspeedConfiguration _configuration;
        private readonly IPropertyResolver _propertyResolver;

        public ElementsResolver(IOptions<EnterspeedConfiguration> configuration, IPropertyResolver propertyResolver)
        {
            _configuration = configuration?.Value;
            _propertyResolver = propertyResolver;
        }

        public List<EntityType> GetAllElementsForPage(DeliveryApiResponse deliveryApiResponse)
        {
            var entityTypes = new List<EntityType>();

            var route = deliveryApiResponse.Response.Route;
            var elements = GetElementObjectArray(route);

            foreach (var element in elements)
            {
                var dataViewDict = GetDataViewDict(element);
                if (dataViewDict != null)
                {
                    var entityType = new EntityType
                    {
                        Meta = GetMetaDataForElement(dataViewDict)
                    };

                    foreach (var key in dataViewDict.Keys)
                    {
                        var property = _propertyResolver.Resolve(key, dataViewDict);
                        if (property != null)
                            entityType.Properties.Add(property);
                    }

                    entityTypes.Add(entityType);
                }
            }

            return entityTypes;
        }

        private EntityTypeMeta GetMetaDataForElement(Dictionary<string, object> dataViewDict)
        {
            if (!dataViewDict.TryGetValue(_configuration.MigrationComponentMetaData,
                    out var componentMetaData)) return null;

            if (componentMetaData is not Dictionary<string, object> componentMetaDataDict ||
                !componentMetaDataDict.TryGetValue("view", out var metaDataViewValue)) return null;

            if (metaDataViewValue is not Dictionary<string, object> metaDataViewDict) return null;

            metaDataViewDict.TryGetValue("metaData", out var metaData);

            if (metaData is not Dictionary<string, object> metaDataDict) return null;

            var metaDataProperty = new EntityTypeMeta();
            var keys = metaDataDict.Keys;

            foreach (var key in keys)
            {
                switch (key)
                {
                    case "sourceEntityAlias":
                        metaDataProperty.SourceEntityAlias = metaDataDict[key].ToString();
                        break;
                    case "sourceEntityName":
                        metaDataProperty.SourceEntityName = metaDataDict[key].ToString();
                        break;
                }
            }

            return metaDataProperty;
        }

        private List<object> GetElementObjectArray(Dictionary<string, object> route)
        {
            // Check that renderings exists
            if (!route.TryGetValue("renderings", out var renderings))
            {
                throw new NullReferenceException($"renderings property not found in deliveryApiResponse {JsonSerializer.Serialize(route)}");
            }

            // Check that renderings is dictionary and view exists
            if (renderings is not Dictionary<string, object> renderingsDict || !renderingsDict.TryGetValue("view", out var view))
            {
                throw new NullReferenceException($"View property not found in deliveryApiResponse {JsonSerializer.Serialize(route)}");
            }

            // Check that view is dictionary and items exists
            if (view is not Dictionary<string, object> viewDict || !viewDict.TryGetValue("items", out var items))
            {
                throw new NullReferenceException($"Items property not found in deliveryApiResponse {JsonSerializer.Serialize(route)}");
            }

            return items as List<object>;
        }

        private Dictionary<string, object> GetDataViewDict(object element)
        {
            if (element is not Dictionary<string, object> elementDict || !elementDict.TryGetValue("data", out var data)) return null;
            if (data is not Dictionary<string, object> dataDictionary || !dataDictionary.TryGetValue("view", out var dataView)) return null;
            if (dataView is not Dictionary<string, object> viewDict) return null;

            return viewDict;
        }
    }
}