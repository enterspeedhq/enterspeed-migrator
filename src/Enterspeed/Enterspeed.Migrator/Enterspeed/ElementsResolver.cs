using System;
using System.Collections.Generic;
using System.Text.Json;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;

namespace Enterspeed.Migrator.Enterspeed
{
    public class ElementsResolver : IElementsResolver
    {
        private readonly EnterspeedConfiguration _configuration;

        public ElementsResolver(EnterspeedConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets meta data objects for elements
        /// </summary>
        /// <param name="deliveryApiResponse"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public List<EntityTypeMeta> GetMetaDataForElements(DeliveryApiResponse deliveryApiResponse)
        {
            var metadataProperties = new List<EntityTypeMeta>();

            var route = deliveryApiResponse.Response.Route;

            // Check that renderings exists
            if (!deliveryApiResponse.Response.Route.TryGetValue("renderings", out var renderings))
            {
                throw new NullReferenceException($"renderings property not found in deliveryApiResponse {JsonSerializer.Serialize(deliveryApiResponse.Response.Route)}");
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

            // Check that view is dictionary and items exists
            if (items is not List<object> itemsList)
            {
                throw new NullReferenceException($"Data property not found in deliveryApiResponse  {JsonSerializer.Serialize(route)}");
            }

            foreach (var item in itemsList)
            {
                if (item is not Dictionary<string, object> itemDict || !itemDict.TryGetValue("data", out var data)) continue;

                if (data is not Dictionary<string, object> dataDictionary || !dataDictionary.TryGetValue("view", out var dataView)) continue;

                if (dataView is not Dictionary<string, object> dataViewDict || !dataViewDict.TryGetValue(_configuration.MigrationComponentMetaData,
                        out var componentMetaData)) continue;

                if (componentMetaData is not Dictionary<string, object> componentMetaDataDict || !componentMetaDataDict.TryGetValue("view", out var metaDataViewValue)) continue;

                if (metaDataViewValue is not Dictionary<string, object> metaDataViewDict) continue;

                metaDataViewDict.TryGetValue("metaData", out var metaData);

                if (metaData is not Dictionary<string, object> metaDataDict) continue;

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

                metadataProperties.Add(metaDataProperty);
            }

            return metadataProperties;
        }

    }
}
