using System;
using System.Collections.Generic;
using System.Text.Json;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;

namespace Enterspeed.Migrator.Enterspeed
{
    public class PagesResolver : IPagesResolver
    {
        private readonly EnterspeedConfiguration _configuration;

        public PagesResolver(EnterspeedConfiguration configuration)
        {
            _configuration = configuration;
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

            // Getting views
            if (migrationPageMetaData is not Dictionary<string, object> migrationPageMetaDataDict || !migrationPageMetaDataDict.TryGetValue("view", out var view))
            {
                throw new NullReferenceException($"View property not found in route {JsonSerializer.Serialize(route)}");
            }

            // Getting page meta data property value
            if (view is not Dictionary<string, object> viewDict || !viewDict.TryGetValue("metaData", out var metaData))
            {
                throw new NullReferenceException($"MetaData property not found in route {JsonSerializer.Serialize(route)}");
            }

            // Get page metadata property and map values
            if (metaData is not Dictionary<string, object> metaDataDict ||
                !metaDataDict.TryGetValue("sourceEntityAlias", out var alias) ||
                !metaDataDict.TryGetValue("sourceEntityName", out var name))
            {
                throw new NullReferenceException($"Meta data values could not be mapped {JsonSerializer.Serialize(route)}");
            }

            return new EntityTypeMeta(alias.ToString(), name.ToString());
        }
    }
}
