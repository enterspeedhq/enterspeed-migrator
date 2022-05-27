using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco9.Migrator.Builders.Contracts;
using Umbraco9.Migrator.DataTypes;
using Umbraco9.Migrator.Settings;

namespace Umbraco9.Migrator.Builders
{
    public class ContentBuilder : IContentBuilder
    {
        private readonly IContentService _contentService;
        private readonly IEnumerable<IContentType> _contentTypes;
        private readonly UmbracoMigrationConfiguration _umbracoMigrationConfiguration;

        public ContentBuilder(IContentTypeService contentTypeService, IContentService contentService, IOptions<UmbracoMigrationConfiguration> umbracoMigrationConfiguration)
        {
            _contentService = contentService;
            _umbracoMigrationConfiguration = umbracoMigrationConfiguration?.Value;
            _contentTypes = contentTypeService.GetAll();
        }

        public void BuildContentPages(List<PageEntityType> pageEntityTypes, IContent parent = null)
        {
            foreach (var pageEntityType in pageEntityTypes)
            {
                var isRoot = pageEntityType.Meta.SourceEntityAlias.Contains(_umbracoMigrationConfiguration.RootDocType);
                var contentType = _contentTypes.FirstOrDefault(c => string.Equals(c.Alias, pageEntityType.Meta.SourceEntityAlias, StringComparison.InvariantCultureIgnoreCase));

                if (parent == null && !isRoot) continue;

                var contentToCreate = _contentService.Create(pageEntityType.Meta.ContentName, isRoot ? -1 : parent.Id, contentType);

                foreach (var property in pageEntityType.Properties)
                {
                    contentToCreate.Properties[property.Alias].SetValue(property.Value);
                }

                var blockList = PopulateBlockList(pageEntityType);
                var blockListSerialized = JsonConvert.SerializeObject(blockList);
                contentToCreate.SetValue(_umbracoMigrationConfiguration.ContentPropertyAlias, blockListSerialized);
                _contentService.Save(contentToCreate);

                if (pageEntityType.Children != null && pageEntityType.Children.Any())
                {
                    BuildContentPages(pageEntityType.Children, contentToCreate);
                }
            }
        }

        private Blocklist PopulateBlockList(PageEntityType pageEntityType)
        {
            var blockListData = new List<Dictionary<string, string>>();
            var dictionaryUdi = new List<Dictionary<string, string>>();
            var settingsList = new List<Dictionary<string, string>>();

            foreach (var element in pageEntityType.Components)
            {
                if (element.Properties == null || element.Meta == null) continue;

                var dataToAdd = new Dictionary<string, string>();
                foreach (var property in element.Properties)
                {
                    dataToAdd.Add(property.Alias.ToFirstLowerInvariant(), property.Value);
                }

                var contentUdi = new GuidUdi("element", Guid.NewGuid()).ToString();
                var settingsUdi = new GuidUdi("element", Guid.NewGuid()).ToString();

                dataToAdd.Add("udi", contentUdi);
                blockListData.Add(dataToAdd);

                dictionaryUdi.Add(new Dictionary<string, string> {
                    { "contentUdi", contentUdi },
                    { "settingsUdi", settingsUdi }
                });

                var contentType = _contentTypes.FirstOrDefault(c => string.Equals(c.Alias, element.Meta.SourceEntityAlias,
                    StringComparison.InvariantCultureIgnoreCase));

                settingsList.Add(new Dictionary<string, string>
                {
                    //in this case the settings is set to use the contentTypeKey as the content - Person document type
                    {"contentTypeKey", contentType.Key.ToString()},
                    {"udi", settingsUdi},
                    //role is our setting for this blocklist - not a property for the actual content
                    {"role", "content editor"}
                });
            }

            return new Blocklist
            {
                layout = new BlockListUdi(dictionaryUdi, settingsList),
                contentData = blockListData,
                settingsData = settingsList
            };
        }
    }
}
