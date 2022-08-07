using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.ValueTypes;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco10.Migrator.DataTypes;
using Umbraco10.Migrator.Settings;

namespace Umbraco10.Migrator.Content
{
    public class ContentBuilder : IContentBuilder
    {
        private readonly IContentService _contentService;
        private readonly IEnumerable<IContentType> _contentTypes;
        private readonly UmbracoMigrationConfiguration _umbracoMigrationConfiguration;

        public ContentBuilder(IContentTypeService contentTypeService,
            IContentService contentService,
            IOptions<UmbracoMigrationConfiguration> umbracoMigrationConfiguration)
        {
            _contentService = contentService;
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

                CreateProperties(pageEntityType.Properties, contentToCreate);

                _contentService.Save(contentToCreate);

                if (pageEntityType.Children != null && pageEntityType.Children.Any())
                {
                    BuildContentPages(pageEntityType.Children, contentToCreate);
                }
            }
        }

        public void CreateProperties(List<EnterspeedPropertyType> properties, IContent contentToCreate, bool isTraversing = false)
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
                }

                if (property.ChildProperties.Any())
                {
                    CreateProperties(property.ChildProperties, contentToCreate, true);
                }
            }
        }

        public void AddValueToProperty(EnterspeedPropertyType property, IContent contentToCreate)
        {
            if (contentToCreate.Properties.FirstOrDefault(p => p.Alias == property.Alias) != null)
            {
                switch (property.Type)
                {
                    case System.Text.Json.JsonValueKind.True:
                    case System.Text.Json.JsonValueKind.False:
                        var value = bool.Parse(property.Value.ToString());
                        contentToCreate.Properties[property.Alias].SetValue(value);
                        break;
                    case System.Text.Json.JsonValueKind.Null:
                        break;
                    default:
                        contentToCreate.Properties[property.Alias].SetValue(property.Value);
                        break;
                }
            }
        }

        private Blocklist PopulateBlockList(PageData pageData)
        {
            var blockListData = new List<Dictionary<string, object>>();
            var dictionaryUdi = new List<Dictionary<string, string>>();

            //foreach (var element in pageData.Components)
            //{
            //    if (element.Properties == null || element.MetaSchema == null) continue;

            //    var dataToAdd = new Dictionary<string, object>();
            //    foreach (var property in element.Properties)
            //    {
            //        dataToAdd.Add(property.Alias.ToFirstLowerInvariant(), property.Value);
            //    }

            //    var contentUdi = new GuidUdi("element", Guid.NewGuid()).ToString();
            //    var contentType = _contentTypes.FirstOrDefault(c => string.Equals(c.Alias,
            //        element.MetaSchema.SourceEntityAlias,
            //        StringComparison.InvariantCultureIgnoreCase));

            //    dataToAdd.Add("udi", contentUdi);
            //    dataToAdd.Add("contentTypeKey", contentType.Key.ToString());
            //    blockListData.Add(dataToAdd);

            //    dictionaryUdi.Add(new Dictionary<string, string>
            //    {
            //        { "contentUdi", contentUdi },
            //    });
            //}

            return new Blocklist
            {
                layout = new BlockListUdi(dictionaryUdi),
                contentData = blockListData,
                settingsData = new List<Dictionary<string, string>>()
            };
        }
    }
}