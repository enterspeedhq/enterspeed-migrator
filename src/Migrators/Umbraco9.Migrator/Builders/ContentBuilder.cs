using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco9.Migrator.Builders.Contracts;

namespace Umbraco9.Migrator.Builders
{
    public class ContentBuilder : IContentBuilder
    {
        private readonly IContentService _contentService;
        private readonly IEnumerable<IContentType> _contentTypes;

        public ContentBuilder(IContentTypeService contentTypeService, IContentService contentService)
        {
            _contentService = contentService;
            _contentTypes = contentTypeService.GetAll();
        }

        public void BuildContentPages(List<PageEntityType> pageEntityTypes, IContent parent = null)
        {
            foreach (var pageEntityType in pageEntityTypes)
            {
                var isRoot = pageEntityType.Meta.SourceEntityAlias.Contains("home");
                var contentType = _contentTypes.FirstOrDefault(c => string.Equals(c.Alias, pageEntityType.Meta.SourceEntityAlias, StringComparison.InvariantCultureIgnoreCase));

                var contentToCreate = _contentService.Create(pageEntityType.Meta.ContentName, isRoot ? -1 : parent.Id, contentType);

                foreach (var property in pageEntityType.Properties)
                {
                    contentToCreate.Properties[property.Alias].SetValue(property.Value);
                }

                _contentService.Save(contentToCreate);

                if (pageEntityType.Children != null && pageEntityType.Children.Any())
                {
                    BuildContentPages(pageEntityType.Children, contentToCreate);
                }
            }
        }
    }
}
