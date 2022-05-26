using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco9.Migrator.Builders.Contracts;
using Umbraco9.Migrator.Extensions;

namespace Umbraco9.Migrator.Builders
{
    public class DocumentTypeBuilder : IDocumentTypeBuilder
    {
        private readonly EnterspeedConfiguration _enterspeedConfiguration;
        private readonly ILogger<DocumentTypeBuilder> _logger;
        private readonly IContentTypeService _contentTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IDataTypeService _dataTypeService;
        private readonly List<ContentTypeSort> _contentTypes;
        private readonly BlockListPropertyEditor _blockListPropertyEditor;
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private ContentType _root;
        const string PagesContainerName = "Migrated Page Types";
        const string ElementsContainerName = "Migrated Elements";
        private const string BlockListName = "BlockList.Custom";
        private readonly IEnumerable<IDataType> _dataTypes;


        public DocumentTypeBuilder(ILogger<DocumentTypeBuilder> logger, IOptions<EnterspeedConfiguration> enterspeedConfiguration, IContentTypeService contentTypeService, IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService, BlockListPropertyEditor blockListPropertyEditor, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _enterspeedConfiguration = enterspeedConfiguration?.Value;
            _logger = logger;
            _contentTypeService = contentTypeService;
            _shortStringHelper = shortStringHelper;
            _dataTypeService = dataTypeService;
            _blockListPropertyEditor = blockListPropertyEditor;
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
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
                    Name = BlockListName,
                    Configuration = new BlockListConfiguration()
                    {
                        Blocks = new List<BlockListConfiguration.BlockConfiguration>()
                        {

                        }.ToArray()
                    }
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
                    Alias = page.Meta.SourceEntityAlias.FirstCharToLower(),
                    Name = page.Meta.SourceEntityName.FirstCharToUpper(),
                    AllowedAsRoot = string.Equals(page.Meta.SourceEntityAlias, _enterspeedConfiguration.RootPageType, StringComparison.InvariantCultureIgnoreCase)
                };

                newPageDocumentType.AddPropertyType(new PropertyType(_shortStringHelper, _dataTypes.FirstOrDefault(d =>
                    d.Name == BlockListName))
                {
                    Name = "Content",
                    Alias = "content"
                });

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
                        Alias = component.Meta.SourceEntityAlias.FirstCharToLower(),
                        Name = component.Meta.SourceEntityName.FirstCharToUpper(),
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
                            dataType = dataTypeDefinitions.FirstOrDefault(d => d.Name.ToLower() == "textarea");
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
                        newComponentDocumentType.AddPropertyType(new PropertyType(_shortStringHelper, dataType)
                        {
                            Name = property.Name.FirstCharToUpper(),
                            Alias = property.Alias.FirstCharToLower(),
                        });
                }
            }
        }

        private void AddElementsToBlockList()
        {
            var blockListType = _dataTypeService.GetDataType(BlockListName);
            var elementTypes = _contentTypeService.GetAll().Where(c => c.IsElement);

            var blocks = new List<BlockListConfiguration.BlockConfiguration>();
            foreach (var block in elementTypes)
            {
                blocks.Add(new BlockListConfiguration.BlockConfiguration()
                {
                    ContentElementTypeKey = block.Key,
                    Label = block.Name
                });
            }

            blockListType.Configuration = new BlockListConfiguration()
            {
                Blocks = blocks.ToArray()
            };

            _dataTypeService.Save(blockListType);
        }
    }
}
