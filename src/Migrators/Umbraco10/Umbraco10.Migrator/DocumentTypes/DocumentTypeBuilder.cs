using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Enterspeed.Migrator.ValueTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco10.Migrator.DocumentTypes.Components;
using Umbraco10.Migrator.Settings;

namespace Umbraco10.Migrator.DocumentTypes
{
    public class DocumentTypeBuilder : IDocumentTypeBuilder
    {
        private readonly ILogger<DocumentTypeBuilder> _logger;
        private readonly IContentTypeService _contentTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IDataTypeService _dataTypeService;
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private readonly IEnumerable<IDataType> _dataTypes;
        private readonly IComponentBuilderHandler _componentBuilderHandler;
        private readonly List<ContentTypeSort> _contentTypes;
        private readonly BlockListPropertyEditor _blockListPropertyEditor;
        private readonly UmbracoMigrationConfiguration _umbracoMigrationConfiguration;
        private readonly IEnumerable<string> _contentTypeAliasList;
        private const string PagesFolderName = "Migrated Page Types";
        private const string ComponentsFolderName = "Migrated Components";
        private const string BlockListName = "BlockList.Custom";
        private readonly EnterspeedConfiguration _enterspeedConfiguration;

        public DocumentTypeBuilder(ILogger<DocumentTypeBuilder> logger,
            IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService,
            BlockListPropertyEditor blockListPropertyEditor,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
            IOptions<UmbracoMigrationConfiguration> umbracoMigrationConfiguration,
            IComponentBuilderHandler componentBuilderHandler,
            IOptions<EnterspeedConfiguration> enterspeedConfiguration)
        {
            _logger = logger;
            _contentTypeService = contentTypeService;
            _shortStringHelper = shortStringHelper;
            _dataTypeService = dataTypeService;
            _blockListPropertyEditor = blockListPropertyEditor;
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
            _umbracoMigrationConfiguration = umbracoMigrationConfiguration?.Value;
            _dataTypes = _dataTypeService.GetAll();
            _contentTypes = new List<ContentTypeSort>();
            _componentBuilderHandler = componentBuilderHandler;
            _contentTypeAliasList = _contentTypeService.GetAllContentTypeAliases();
            _enterspeedConfiguration = enterspeedConfiguration?.Value;
        }

        public void BuildDocTypes(Schemas schemas)
        {
            try
            {
                var pagesFolder = GetOrCreateFolder(PagesFolderName);
                var componentsFolder = GetOrCreateFolder(ComponentsFolderName);

                var sortOrder = 0;
                foreach (var page in schemas.Pages)
                {
                    CreatePageDocType(page, pagesFolder, sortOrder);
                    sortOrder++;
                }

                foreach (var componentAlias in _enterspeedConfiguration.ComponentPropertyTypeKeys)
                {
                    _componentBuilderHandler.BuildComponent(componentAlias, componentsFolder.Id);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when building schemas");
                throw;
            }
        }

        private void CreatePageDocType(Schema page, IEntity pagesFolder, int sortOrder)
        {
            if (!_contentTypeAliasList.Any(c => c.Equals(page.MetaSchema.SourceEntityAlias)))
            {
                // Build new doc type
                var newPageDocumentType = BuildNewPageDocType(page, pagesFolder);

                // Add base properties
                AddBaseProperties(newPageDocumentType, sortOrder);

                // Add the rest of the properties
                AddProperties(page, newPageDocumentType);

                // Save the new document type
                _contentTypeService.Save(newPageDocumentType);
            }
        }

        private void AddBaseProperties(ContentType newPageDocumentType, int sortOrder)
        {
            if (!newPageDocumentType.AllowedAsRoot)
            {
                _contentTypes.Add(new ContentTypeSort(newPageDocumentType.Id, sortOrder));
            }
        }

        private ContentType BuildNewPageDocType(Schema page, IEntity pagesFolder)
        {
            var newPageDocumentType = new ContentType(_shortStringHelper, pagesFolder.Id)
            {
                Alias = page.MetaSchema.SourceEntityAlias.ToFirstLowerInvariant(),
                Name = page.MetaSchema.SourceEntityName.ToFirstUpperInvariant(),
                AllowedAsRoot = string.Equals(page.MetaSchema.SourceEntityAlias, _umbracoMigrationConfiguration.RootDocType,
                    StringComparison.InvariantCultureIgnoreCase)
            };

            return newPageDocumentType;
        }

        private void AddProperties(Schema schema, IContentTypeBase pageDocumentType)
        {
            if (_dataTypes != null && _dataTypes.Any())
            {
                foreach (var property in schema.Properties)
                {
                    AddProperties(property, pageDocumentType);
                }
            }
        }

        private void AddProperties(EnterspeedPropertyType enterspeedProperty, IContentTypeBase pageDocumentType)
        {
            IDataType dataType = null;

            switch (enterspeedProperty.Type)
            {
                case JsonValueKind.Undefined:
                    _logger.LogError("Property type is undefined");
                    break;
                case JsonValueKind.Object:
                    // Check if we are a component
                    break;
                case JsonValueKind.Array:
                    // Check if we are a component or simple type of array
                    break;
                case JsonValueKind.String:
                    dataType = _dataTypes.FirstOrDefault(d => d.Name?.ToLower() == "textstring");
                    break;
                case JsonValueKind.Number:
                    dataType = _dataTypes.FirstOrDefault(d => d.Name?.ToLower() == "number");
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    dataType = _dataTypes.FirstOrDefault(d => d.Name?.ToLower() == "true/false");
                    break;
                case JsonValueKind.Null:
                    _logger.LogError("Property type is null");
                    break;
            }

            if (dataType != null)
            {
                pageDocumentType.AddPropertyType(new PropertyType(_shortStringHelper, dataType)
                {
                    Name = enterspeedProperty.Name.ToFirstUpperInvariant(),
                    Alias = enterspeedProperty.Alias.ToFirstLowerInvariant(),
                }, "content", "Page content");
            }
        }

        private EntityContainer GetOrCreateFolder(string folderName)
        {
            var createContainerAttempt = _contentTypeService.CreateContainer(-1, Guid.NewGuid(), folderName);
            if (createContainerAttempt.Success)
            {
                var containerSaved = _contentTypeService.SaveContainer(createContainerAttempt.Result.Entity);
                if (containerSaved.Success)
                {
                    return createContainerAttempt.Result.Entity;
                }

                _logger.LogError(containerSaved.Exception, $"Something went wrong when saving folder {folderName}");
            }

            var existingContainer = _contentTypeService.GetContainers(folderName, 1).FirstOrDefault();
            return existingContainer;
        }

        private void AddComponentsToBlockList()
        {
            var dataType = new DataType(_blockListPropertyEditor, _configurationEditorJsonSerializer)
            {
                Name = BlockListName
            };

            _dataTypeService.Save(dataType);

            var blockListType = _dataTypeService.GetDataType(BlockListName);
            var elementTypes = _contentTypeService.GetAll().Where(c => c.IsElement);

            blockListType.Configuration = new BlockListConfiguration()
            {
                Blocks = elementTypes.Select(block =>
                    new BlockListConfiguration.BlockConfiguration()
                    {
                        ContentElementTypeKey = block.Key,
                        Label = block.Name
                    }).ToArray()
            };

            _dataTypeService.Save(blockListType);
        }
    }
}