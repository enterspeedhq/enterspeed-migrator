using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Umbraco10.Migrator.DocumentTypes.Components.Builders
{
    public interface IComponentBuilder
    {
        bool CanBuild(string propertyAlias);
        void Build();
        IComponentBuilder Populate(EnterspeedPropertyType componentProperty, List<EnterspeedPropertyType> componentProperties, int parentFolderId);
        bool ComponentExists(EnterspeedPropertyType componentProperty);
    }
}