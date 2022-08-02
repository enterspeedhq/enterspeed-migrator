using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
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
        private readonly List<ContentTypeSort> _contentTypes;
        private readonly BlockListPropertyEditor _blockListPropertyEditor;
        private readonly UmbracoMigrationConfiguration _umbracoMigrationConfiguration;
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
            IOptions<UmbracoMigrationConfiguration> umbracoMigrationConfiguration)
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
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();
            if (!contentTypes.Any(c => c.Equals(page.MetaSchema.SourceEntityAlias)))
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

                _contentTypeService.Save(newPageDocumentType);

                if (newPageDocumentType.AllowedAsRoot)
                {
                    _root = newPageDocumentType;
                }
                else
                {
                    _contentTypes.Add(new ContentTypeSort(newPageDocumentType.Id, sortOrder));
                }
            }
        }

        public void CreateComponents(Schemas schemas)
        {
            var componentsFolder = GetOrCreateFolder(ComponentsFolderName);
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();

            foreach (var component in schemas.Components)
            {
                if (!contentTypes.Any(c => c.Equals(component.MetaSchema.SourceEntityAlias)))
                {
                    var newComponentDocumentType = new ContentType(_shortStringHelper, componentsFolder.Id)
                    {
                        Alias = component.MetaSchema.SourceEntityAlias.ToFirstLowerInvariant(),
                        Name = component.MetaSchema.SourceEntityName.ToFirstUpperInvariant(),
                        IsElement = true
                    };

                    AddPropertyTypes(component, newComponentDocumentType);

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

        private void AddPropertyTypes(Schema schema, ContentType newComponentDocumentType)
        {
            var dataTypeDefinitions = _dataTypes;
            if (dataTypeDefinitions != null && dataTypeDefinitions.Any())
            {
                foreach (var property in schema.Properties)
                {
                    IDataType dataType;
                    switch (property.Type.ToLowerInvariant())
                    {
                        case "text":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "textstring");
                            break;
                        case "textarea":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "textarea");
                            break;
                        case "rte":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "richtext editor");
                            break;
                        case "image":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "textstring");
                            break;
                        case "boolean":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "true/false");
                            break;
                        case "link":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "textstring");
                            break;
                        default:
                            dataType = null;
                            break;
                    }

                    if (dataType != null)
                    {
                        newComponentDocumentType.AddPropertyGroup("componentData", "Component Data");
                        newComponentDocumentType.AddPropertyType(new PropertyType(_shortStringHelper, dataType)
                        {
                            Name = property.Name.ToFirstUpperInvariant(),
                            Alias = property.Alias.ToFirstLowerInvariant(),
                        }, "componentData", "Component Data");
                    }
                }
            }
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