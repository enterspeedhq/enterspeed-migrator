using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.ValueTypes;
using Microsoft.Extensions.Logging;
using Umbraco10.Migrator.DocumentTypes.Components.Contracts;

namespace Umbraco10.Migrator.DocumentTypes.Components
{
    public class ComponentBuilderHandler : IComponentBuilderHandler
    {
        private readonly IEnumerable<IComponentBuilder> _componentBuilders;
        private readonly ILogger<ComponentBuilderHandler> _logger;

        public ComponentBuilderHandler(
            IEnumerable<IComponentBuilder> componentBuilders,
            ILogger<ComponentBuilderHandler> logger)
        {
            _componentBuilders = componentBuilders;
            _logger = logger;
        }

        public void BuildComponent(PropertyType propertyType, int parentId)
        {
            var name = propertyType.ChildProperties.FirstOrDefault(p => p.Name == "name")?.Value.ToString();
            var alias = propertyType.ChildProperties.FirstOrDefault(p => p.Name == "alias")?.Value.ToString();

            if (string.IsNullOrEmpty(alias)) return;

            var componentBuilder = _componentBuilders.FirstOrDefault(p => p.CanBuild(alias));
            if (componentBuilder != null)
            {
                componentBuilder.Build(propertyType, parentId);
            }
            else
            {
                _logger.LogError("No component property converter found for " + name);
            }
        }
    }
}