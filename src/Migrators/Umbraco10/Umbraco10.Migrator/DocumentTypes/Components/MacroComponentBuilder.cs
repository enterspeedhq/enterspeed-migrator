using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco10.Migrator.DocumentTypes.Components.Contracts;
using EnterspeedPropertyType = Enterspeed.Migrator.ValueTypes.PropertyType;

namespace Umbraco10.Migrator.DocumentTypes.Components

{
    public class MacroComponentBuilder : IComponentBuilder
    {
        private const string PropertyAlias = "macro";
        private const string PropertyGroupAlias = "componentData";
        private const string PropertyGroupName = "componentData";
        private readonly IContentTypeService _contentTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IDataTypeService _dataTypeService;

        public MacroComponentBuilder(IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService)
        {
            _contentTypeService = contentTypeService;
            _shortStringHelper = shortStringHelper;
            _dataTypeService = dataTypeService;
        }

        public bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public void Build(EnterspeedPropertyType componentProperty, int parentFolderId)
        {
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();

            if (contentTypes.Any(c => c.Equals(componentProperty.Alias))) return;

            var newComponentDocumentType = new ContentType(_shortStringHelper, parentFolderId)
            {
                Alias = componentProperty.Alias.ToFirstLowerInvariant(),
                Name = componentProperty.Name.ToFirstUpperInvariant(),
                IsElement = true
            };

            var rteProperty = componentProperty.ChildProperties.FirstOrDefault(p => p.Name == "value");

            newComponentDocumentType.AddPropertyGroup(PropertyGroupAlias, PropertyGroupName);
            newComponentDocumentType.AddPropertyType(
                new PropertyType(_shortStringHelper, _dataTypeService.GetDataType(Constants.DataTypes.RichtextEditor))
                {
                    Alias = rteProperty.Alias, Name = rteProperty.Name
                }, PropertyGroupAlias, PropertyGroupName);

            _contentTypeService.Save(newComponentDocumentType);
        }
    }
}