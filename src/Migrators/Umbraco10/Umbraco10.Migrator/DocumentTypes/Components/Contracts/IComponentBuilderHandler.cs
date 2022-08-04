using Enterspeed.Migrator.ValueTypes;

namespace Umbraco10.Migrator.DocumentTypes.Components.Contracts
{
    public interface IComponentBuilderHandler
    {
        void BuildComponent(PropertyType propertyType, int parentId);
    }
}