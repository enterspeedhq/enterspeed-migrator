using EnterspeedPropertyType = Enterspeed.Migrator.ValueTypes.PropertyType;

namespace Umbraco10.Migrator.DocumentTypes.Components.Contracts
{
    public interface IComponentBuilder
    {
        public bool CanBuild(string propertyAlias);
        public void Build(EnterspeedPropertyType componentProperty, int parentFolderId);
    }
}