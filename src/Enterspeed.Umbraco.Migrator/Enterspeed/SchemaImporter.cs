using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Umbraco.Migrator.Enterspeed
{
    public class SchemaImporter : ISchemaImporter
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SchemaImporter> _logger;

        public SchemaImporter(IApiService apiService, ILogger<SchemaImporter> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<DocumentTypes> ImportSchemasAsync()
        {
            var navigation = await _apiService.GetNavigationAsync();
            var urls = new List<string>();
            foreach (var child in navigation.Views.Navigation.Children)
            {
                AddUrl(child, urls);
            }

            var documentTypes = new DocumentTypes();
            var responses = new List<DeliveryApiResponse>();

            foreach (var url in urls)
            {
                var response = await _apiService.GetByUrlsAsync(url);
                responses.Add(response);
            }

            try
            {
                foreach (var response in responses)
                {
                    var baseProperties = GetPageBaseProperties(response, documentTypes);
                    if (documentTypes.Pages.All(p => p.Alias != baseProperties.Alias))
                    {
                        documentTypes.Pages.Add(new DocumentType
                        {
                            Alias = baseProperties.Alias,
                            Name = baseProperties.Name
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when migrating schemas");
                throw;
            }

            return documentTypes;
        }

        private DocumentTypeBaseProperties GetPageBaseProperties(DeliveryApiResponse response, DocumentTypes documentTypes)
        {
            if (!response.Response.Route.TryGetValue("pageDocType", out object pageDocType))
            {
                throw new NullReferenceException("pageDocType not found on schema for " + JsonSerializer.Serialize(pageDocType));
            }

            if (pageDocType is not Dictionary<string, object> pageDocTypeDict ||
                !pageDocTypeDict.TryGetValue("view", out object view))
            {
                throw new NullReferenceException("pageDocType view property not found on schema " + JsonSerializer.Serialize(pageDocType));
            }

            if (view is not Dictionary<string, object> viewDict ||
                !viewDict.TryGetValue("documentType", out object docType))
            {
                throw new NullReferenceException("document type property not found on schema " + JsonSerializer.Serialize(pageDocType));
            }


            if (docType is not Dictionary<string, object> docTypeDict ||
                !docTypeDict.TryGetValue("alias", out object alias) ||
                !docTypeDict.TryGetValue("name", out object name))
            {
                throw new NullReferenceException("alias or name property not found on schema " + JsonSerializer.Serialize(pageDocType));
            }

            return new DocumentTypeBaseProperties
            {
                Alias = alias.ToString(),
                Name = name.ToString()
            };
        }

        private void AddUrl(Child child, List<string> urls)
        {
            urls.Add(child.View.Self.View.Url);
            if (child.View.Children.Any())
            {
                foreach (var subChild in child.View.Children)
                {
                    AddUrl(subChild, urls);
                }
            }
        }
    }
}