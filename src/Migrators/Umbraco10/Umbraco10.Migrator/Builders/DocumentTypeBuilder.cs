using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco10.Migrator.Builders.Contracts;
using Umbraco10.Migrator.Settings;

namespace Umbraco10.Migrator.Builders
{
    public class DocumentTypeBuilder : IDocumentTypeBuilder
    {
        private readonly ILogger<DocumentTypeBuilder> _logger;
        private readonly IContentTypeService _contentTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IDataTypeService _dataTypeService;
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private readonly IEnumerable<IDataType> _dataTypes;
        private readonly EnterspeedConfiguration _enterspeedConfiguration;
        private readonly List<ContentTypeSort> _contentTypes;
        private readonly BlockListPropertyEditor _blockListPropertyEditor;
        private readonly UmbracoMigrationConfiguration _umbracoMigrationConfiguration;
        private readonly IEnumerable<string> _contentTypeAliasList;
        private ContentType _root;
        private const string PagesFolderName = "Migrated Page Types";
        private const string ComponentsFolderName = "Migrated Components";
        private const string BlockListName = "BlockList.Custom";

        public DocumentTypeBuilder(ILogger<DocumentTypeBuilder> logger,
            IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService,
            BlockListPropertyEditor blockListPropertyEditor,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
            IOptions<UmbracoMigrationConfiguration> umbracoMigrationConfiguration,
            EnterspeedConfiguration enterspeedConfiguration)
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
            _enterspeedConfiguration = enterspeedConfiguration;
            _contentTypeAliasList = _contentTypeService.GetAllContentTypeAliases();
        }

        public void BuildPageDocTypes(Schemas schemas)
        {
            try
            {
                var pagesFolder = GetOrCreateFolder(PagesFolderName);
                var dataType = new DataType(_blockListPropertyEditor, _configurationEditorJsonSerializer)
                {
                    Name = BlockListName
                };

                _dataTypeService.Save(dataType);

                var sortOrder = 0;
                foreach (var page in schemas.Pages)
                {
                    CreatePageDocType(page, pagesFolder, sortOrder);
                    sortOrder++;
                }

                _root.AllowedContentTypes = _contentTypes;
                _contentTypeService.Save(_root);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when building schemas");
                throw;
            }
        }

        private void CreatePageDocType(Schema page, EntityContainer container, int sortOrder)
        {
            if (!_contentTypeAliasList.Any(c => c.Equals(page.MetaSchema.SourceEntityAlias)))
            {
                var newPageDocumentType = new ContentType(_shortStringHelper, container.Id)
                {
                    Alias = page.MetaSchema.SourceEntityAlias.ToFirstLowerInvariant(),
                    Name = page.MetaSchema.SourceEntityName.ToFirstUpperInvariant(),
                    AllowedAsRoot = string.Equals(page.MetaSchema.SourceEntityAlias, _umbracoMigrationConfiguration.RootDocType,
                        StringComparison.InvariantCultureIgnoreCase)
                };

                newPageDocumentType.AddPropertyGroup("pageContent", "Page Content");
                newPageDocumentType.AddPropertyType(new PropertyType(_shortStringHelper, _dataTypes.FirstOrDefault(d => d.Name == BlockListName))
                {
                    Name = _umbracoMigrationConfiguration.ContentPropertyAlias.ToFirstUpperInvariant(),
                    Alias = _umbracoMigrationConfiguration.ContentPropertyAlias.ToFirstLowerInvariant()
                }, "pageContent");


                if (newPageDocumentType.AllowedAsRoot)
                {
                    _root = newPageDocumentType;
                }
                else
                {
                    _contentTypes.Add(new ContentTypeSort(newPageDocumentType.Id, sortOrder));
                }

                AddPropertyTypes(page, newPageDocumentType);
                _contentTypeService.Save(newPageDocumentType);
            }
        }

        private void AddPropertyTypes(Schema schema, ContentType pageDocumentType)
        {
            if (_dataTypes != null && _dataTypes.Any())
            {
                foreach (var property in schema.Properties)
                {
                    if (property.IsComponent(_enterspeedConfiguration))
                    {
                    }
                    else
                    {
                        AddPropertyTypes(property, pageDocumentType);
                    }
                }
            }
        }

        private void AddPropertyTypes(Enterspeed.Migrator.ValueTypes.PropertyType property, ContentType pageDocumentType)
        {
            IDataType dataType = null;

            var jsonElement = (JsonElement)property.Value;
            switch (jsonElement.ValueKind)
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
                    dataType = _dataTypes.FirstOrDefault(d => d.Name.ToLower() == "textstring");
                    break;
                case JsonValueKind.Number:
                    dataType = _dataTypes.FirstOrDefault(d => d.Name.ToLower() == "number");
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    dataType = _dataTypes.FirstOrDefault(d => d.Name.ToLower() == "true/false");
                    break;
                case JsonValueKind.Null:
                    _logger.LogError("Property type is null");
                    break;
            }
            if (dataType != null)
            {
                pageDocumentType.AddPropertyType(new PropertyType(_shortStringHelper, dataType)
                {
                    Name = property.Name.ToFirstUpperInvariant(),
                    Alias = property.Alias.ToFirstLowerInvariant(),
                }, "content", "Page content");
            }
        }

        public void AddComponent(Schemas schemas)
        {
            var componentsFolder = GetOrCreateFolder(ComponentsFolderName);

            foreach (var component in schemas.Components)
            {
                if (!_contentTypeAliasList.Any(c => c.Equals(component.MetaSchema.SourceEntityAlias)))
                {
                    var newComponentDocumentType = new ContentType(_shortStringHelper, componentsFolder.Id)
                    {
                        Alias = component.MetaSchema.SourceEntityAlias.ToFirstLowerInvariant(),
                        Name = component.MetaSchema.SourceEntityName.ToFirstUpperInvariant(),
                        IsElement = true
                    };

                    _contentTypeService.Save(newComponentDocumentType);
                }
            }

            AddComponentsToBlockList();
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

                _logger.LogError(containerSaved.Exception, "Something went wrong when saving folder " + folderName);
            }

            var existingContainer = _contentTypeService.GetContainers(PagesFolderName, 1).FirstOrDefault();
            return existingContainer;
        }

        private void AddComponentsToBlockList()
        {
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