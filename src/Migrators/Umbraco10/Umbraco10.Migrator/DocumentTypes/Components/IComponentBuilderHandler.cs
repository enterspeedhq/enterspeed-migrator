using Enterspeed.Migrator.ValueTypes;

namespace Umbraco10.Migrator.DocumentTypes.Components
{
    public interface IComponentBuilderHandler
    {
        void BuildComponent(EnterspeedPropertyType componentProperty, int parentId);
    }
}