using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco9.Migrator.Builders.Contracts;
using Umbraco9.Migrator.Extensions;

namespace Umbraco9.Migrator.Builders
{
    public class ContentBuilder : IContentBuilder
    {
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;

        public ContentBuilder(IContentTypeService contentTypeService, IContentService contentService)
        {
            _contentTypeService = contentTypeService;
            _contentService = contentService;
        }

        public void BuildContentPages(List<PageEntityType> pageEntityTypes)
        {
            var contentTypes = _contentTypeService.GetAll();

            IContent rootContent = null;

            foreach (var pageEntityType in pageEntityTypes)
            {
                if (rootContent == null)
                    rootContent = _contentService.GetRootContent().FirstOrDefault();

                var contentType = contentTypes.FirstOrDefault(p =>
                    p.Alias == pageEntityType.Meta.SourceEntityAlias.FirstCharToLower());

                var parentId = contentType.AllowedAsRoot ? -1 : rootContent.Id;

                _contentService.Create(pageEntityType.Meta.SourceEntityName, parentId, contentType);
            }
        }
    }
}
