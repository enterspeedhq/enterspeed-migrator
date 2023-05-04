namespace Umbraco.Migrator.DocumentTypes.Components
{
    public interface IComponentBuilderHandler
    {
        void BuildComponent(string alias, int parentId);
    }
}