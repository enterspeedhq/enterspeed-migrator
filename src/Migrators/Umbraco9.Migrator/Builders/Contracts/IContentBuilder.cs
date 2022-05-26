using System.Collections.Generic;
using Enterspeed.Migrator.Models;

namespace Umbraco9.Migrator.Builders.Contracts
{
    public interface IContentBuilder
    {
        void BuildContentPages(List<PageEntityType> pageEntityTypes);
    }
}