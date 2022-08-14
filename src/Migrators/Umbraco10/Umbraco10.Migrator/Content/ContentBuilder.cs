using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Enterspeed.Migrator.Constants;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.ValueTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco10.Migrator.DataTypes;
using Umbraco10.Migrator.DocumentTypes.Components.Builders;
using Umbraco10.Migrator.Settings;

namespace Umbraco10.Migrator.Content
{
    public class ContentBuilder : IContentBuilder
    {
        private readonly IContentService _contentService;
        private readonly IEnumerable<IContentType> _contentTypes;
        private readonly UmbracoMigrationConfiguration _umbracoMigrationConfiguration;
        private readonly ILogger<ContentBuilder> _logger;
        private readonly IEnumerable<IComponentBuilder> _componentBuilders;

        public ContentBuilder(IContentTypeService contentTypeService,
            IContentService contentService,
            IOptions<UmbracoMigrationConfiguration> umbracoMigrationConfiguration,
            ILogger<ContentBuilder> logger,
            IEnumerable<IComponentBuilder> componentBuilders)
        {
            _contentService = contentService;
            _logger = logger;
            _componentBuilders = componentBuilders;
            _umbracoMigrationConfiguration = umbracoMigrationConfiguration?.Value;
            _contentTypes = contentTypeService.GetAll();
        }

        public void BuildContentPages(List<PageData> pageEntityTypes, IContent parent = null)
        {
            foreach (var pageEntityType in pageEntityTypes)
            {
                var isRoot = pageEntityType.MetaSchema.SourceEntityAlias.Contains(_umbracoMigrationConfiguration.RootDocType);
                var contentType = _contentTypes.FirstOrDefault(c =>
                    string.Equals(c.Alias, pageEntityType.MetaSchema.SourceEntityAlias, StringComparison.InvariantCultureIgnoreCase));

                if (parent == null && !isRoot) continue;

                var contentToCreate = _contentService.Create(pageEntityType.MetaSchema.ContentName, isRoot ? -1 : parent.Id, contentType);

                var components = new List<EnterspeedPropertyType>();
                CreateProperties(pageEntityType.Properties, contentToCreate, components);

                PopulateBlockList(components, contentToCreate);

                _contentService.Save(contentToCreate);

                if (pageEntityType.Children != null && pageEntityType.Children.Any())
                {
                    BuildContentPages(pageEntityType.Children, contentToCreate);
                }
            }
        }

        public void CreateProperties(List<EnterspeedPropertyType> properties, IContent contentToCreate, List<EnterspeedPropertyType> components, bool isTraversing = false)
        {
            foreach (var property in properties)
            {
                // Since we are not traversing and looking for components, we want to add the value to the property found.
                if (!isTraversing)
                {
                    AddValueToProperty(property, contentToCreate);
                }

                // A component has been found, which will be resolved, values assigned to properties and added to list property that is set up in umbraco
                if (property.IsComponent())
                {
                    components.Add(property);
                    continue;
                }

                if (property.ChildProperties.Any())
                {
                    CreateProperties(property.ChildProperties, contentToCreate, components, true);
                }
            }
        }

        public void AddValueToProperty(EnterspeedPropertyType property, IContent contentToCreate)
        {
            if (contentToCreate.Properties.FirstOrDefault(p => p.Alias == property.Alias) != null)
            {
                switch (property.Type)
                {
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        var value = bool.Parse(property.Value.ToString());
                        contentToCreate.Properties[property.Alias].SetValue(value);
                        break;
                    case JsonValueKind.Null:
                        break;
                    default:
                        contentToCreate.Properties[property.Alias].SetValue(property.Value);
                        break;
                }
            }
        }

        private void PopulateBlockList(List<EnterspeedPropertyType> components, IContent contentToCreate)
        {
            // Prepare block list data structure
            var blockListData = new List<Dictionary<string, object>>();
            var dictionaryUdi = new List<Dictionary<string, string>>();

            foreach (var component in components)
            {
                var componentAlias = component.ChildProperties.FirstOrDefault(p => p.Name == EnterspeedPropertyConstants.AliasOf.Alias).Value.ToString();

                if (string.IsNullOrEmpty(componentAlias))
                {
                    _logger.LogError("Component alias was not found for " + System.Text.Json.JsonSerializer.Serialize(component));
                    return;
                }

                // Build component
                var componentBuilder = _componentBuilders.FirstOrDefault(p => p.CanBuild(componentAlias));
                if (componentBuilder != null)
                {
                    var dataToAdd = (Dictionary<string, object>)componentBuilder.MapData(component);

                    var contentUdi = new GuidUdi("element", Guid.NewGuid()).ToString();
                    var contentType = _contentTypes.FirstOrDefault(c => string.Equals(c.Alias,
                        componentAlias,
                        StringComparison.InvariantCultureIgnoreCase));

                    dataToAdd.Add("udi", contentUdi);
                    dataToAdd.Add("contentTypeKey", contentType.Key.ToString());
                    blockListData.Add(dataToAdd);

                    dictionaryUdi.Add(new Dictionary<string, string>
                    {
                        { "contentUdi", contentUdi },
                    });
                }
            }

            var blockList = new Blocklist
            {
                layout = new BlockListUdi(dictionaryUdi),
                contentData = blockListData,
                settingsData = new List<Dictionary<string, string>>()
            };

            var blockListSerialized = JsonConvert.SerializeObject(blockList);

            contentToCreate.SetValue(_umbracoMigrationConfiguration.ContentPropertyAlias, blockListSerialized);
            _contentService.Save(contentToCreate);
        }
    }
}