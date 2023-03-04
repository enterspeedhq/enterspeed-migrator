using System.Collections.Generic;
using Enterspeed.Migrator.Models;
using Umbraco.Cms.Core.Models;

namespace Umbraco.Migrator.Content
{
    public interface IContentBuilder
    {
        void BuildContentPagesInSection(List<PageData> pageEntityTypes, IContent sectionNode);
        void BuildContentPages(List<PageData> pageEntityTypes, IContent parent = null);
    }
}