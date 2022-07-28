using System;
using System.Collections.Generic;
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

        public PagesResolver(IOptions<EnterspeedConfiguration> configuration)
        {
            _configuration = configuration?.Value;
        }

        public List<IPropertyType> GetAllPropertyTypesForPage()
        {
            return new List<IPropertyType>();
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
    }
}