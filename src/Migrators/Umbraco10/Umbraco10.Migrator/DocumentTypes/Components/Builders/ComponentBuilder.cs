using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Constants;
using Enterspeed.Migrator.ValueTypes;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco10.Migrator.DocumentTypes.Components.Builders
{
    public abstract class ComponentBuilder : IComponentBuilder
    {
        private readonly IShortStringHelper _shortStringHelper;
        private const string PropertyGroupAlias = "componentData";
        private const string PropertyGroupName = "Component Data";
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private ContentType _componentDocType;

        protected ComponentBuilder(IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService)
        {
            _contentTypeService = contentTypeService;
            _shortStringHelper = shortStringHelper;
            _dataTypeService = dataTypeService;
        }

        public abstract bool CanBuild(string propertyAlias);

        public abstract void Build();

        public IComponentBuilder Populate(EnterspeedPropertyType componentProperty, List<EnterspeedPropertyType> componentProperties, int parentFolderId)
        {
            _componentDocType = new ContentType(_shortStringHelper, parentFolderId)
            {
                Alias = componentProperties.FirstOrDefault(p => p.Alias == EnterspeedPropertyConstants.AliasOf.Alias).Value.ToString().ToFirstLowerInvariant(),
                Name = componentProperties.FirstOrDefault(p => p.Alias == EnterspeedPropertyConstants.AliasOf.Name).Value.ToString(),
                IsElement = true
            };

            return this;
        }

        public bool ComponentExists(EnterspeedPropertyType componentProperty)
        {
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();
            return contentTypes.Any(c => c.Equals(componentProperty.Alias));
        }

        protected void AddProperty(string alias, string name, int dataTypeId)
        {

            _componentDocType.AddPropertyType(
                new PropertyType(_shortStringHelper, _dataTypeService.GetDataType(dataTypeId))
                {
                    Alias = alias,
                    Name = name
                }, PropertyGroupAlias, PropertyGroupName);
        }

        protected void Save()
        {
            _contentTypeService.Save(_componentDocType);
        }
    }
}