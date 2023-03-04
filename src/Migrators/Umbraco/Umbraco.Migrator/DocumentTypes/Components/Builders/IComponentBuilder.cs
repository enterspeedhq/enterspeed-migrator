using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Umbraco.Migrator.DocumentTypes.Components.Builders
{
    public interface IComponentBuilder
    {
        bool CanBuild(string propertyAlias);
        void Build();
        IComponentBuilder Populate(int parentFolderId);
        bool ComponentExists(string alias);
        object MapData(EnterspeedPropertyType enterspeedProperty);
    }
}