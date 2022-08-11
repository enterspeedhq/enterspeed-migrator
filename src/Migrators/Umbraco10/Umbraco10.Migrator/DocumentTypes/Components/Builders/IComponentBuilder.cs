namespace Umbraco10.Migrator.DocumentTypes.Components.Builders
{
    public interface IComponentBuilder
    {
        bool CanBuild(string propertyAlias);
        void Build();
        IComponentBuilder Populate(int parentFolderId);
        bool ComponentExists(string alias);
    }
}