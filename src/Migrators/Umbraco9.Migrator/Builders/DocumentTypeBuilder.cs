using Enterspeed.Migrator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco9.Migrator.Builders.Contracts;
using Umbraco9.Migrator.Settings;

namespace Umbraco9.Migrator.Builders
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
        const string PagesContainerName = "Migrated Page Types";
        const string ElementsContainerName = "Migrated Elements";
        private const string BlockListName = "BlockList.Custom";


        public DocumentTypeBuilder(ILogger<DocumentTypeBuilder> logger, IContentTypeService contentTypeService, IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService, BlockListPropertyEditor blockListPropertyEditor, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
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

        public void BuildPageDocTypes(EntityTypes entityTypes)
        {
            try
            {
                var container = CreatePagesContainer();
                var sortOrder = 0;

                var dataType = new DataType(_blockListPropertyEditor, _configurationEditorJsonSerializer)
                {
                    Name = BlockListName
                };

                _dataTypeService.Save(dataType);

                foreach (var page in entityTypes.Pages)
                {
                    CreatePageDocType(page, container, sortOrder);
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

        private void CreatePageDocType(EntityType page, Attempt<OperationResult<OperationResultType, EntityContainer>> container, int sortOrder)
        {
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();
            if (!contentTypes.Any(c => c.Equals(page.Meta.SourceEntityAlias)))
            {
                var newPageDocumentType = new ContentType(_shortStringHelper, container.Result.Entity.Id)
                {
                    Alias = page.Meta.SourceEntityAlias.ToFirstLowerInvariant(),
                    Name = page.Meta.SourceEntityName.ToFirstUpperInvariant(),
                    AllowedAsRoot = string.Equals(page.Meta.SourceEntityAlias, _umbracoMigrationConfiguration.RootDocType, StringComparison.InvariantCultureIgnoreCase)
                };

                newPageDocumentType.AddPropertyGroup("pageContent", "Page Content");
                newPageDocumentType.AddPropertyType(new PropertyType(_shortStringHelper,
                    _dataTypes.FirstOrDefault(d => d.Name == BlockListName))
                {
                    Name = "Content",
                    Alias = "content",
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

        private Attempt<OperationResult<OperationResultType, EntityContainer>> CreatePagesContainer()
        {
            var container = _contentTypeService.CreateContainer(-1, Guid.NewGuid(), PagesContainerName);
            _contentTypeService.SaveContainer(container.Result.Entity);
            return container;
        }

        public void CreateElementTypes(EntityTypes entityTypes)
        {
            var container = CreateElementsContainer();
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();


            foreach (var component in entityTypes.Components)
            {
                if (!contentTypes.Any(c => c.Equals(component.Meta.SourceEntityAlias)))
                {
                    var newComponentDocumentType = new ContentType(_shortStringHelper, container.Result.Entity.Id)
                    {
                        Alias = component.Meta.SourceEntityAlias.ToFirstLowerInvariant(),
                        Name = component.Meta.SourceEntityName.ToFirstUpperInvariant(),
                        IsElement = true
                    };

                    AddPropertyTypes(component, newComponentDocumentType);

                    _contentTypeService.Save(newComponentDocumentType);
                }
            }

            AddElementsToBlockList();
        }
        private Attempt<OperationResult<OperationResultType, EntityContainer>> CreateElementsContainer()
        {
            var container = _contentTypeService.CreateContainer(-1, Guid.NewGuid(), ElementsContainerName);
            _contentTypeService.SaveContainer(container.Result.Entity);
            return container;
        }

        private void AddPropertyTypes(EntityType component, ContentType newComponentDocumentType)
        {
            var dataTypeDefinitions = _dataTypes;
            if (dataTypeDefinitions != null && dataTypeDefinitions.Any())
            {
                foreach (var property in component.Properties)
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
                                d.Name.ToLower() == "media picker");
                            break;
                        case "boolean":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "true/false");
                            break;
                        case "link":
                            dataType = dataTypeDefinitions.FirstOrDefault(d =>
                                d.Name.ToLower() == "multi url picker");
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

        private void AddElementsToBlockList()
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
