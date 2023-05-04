using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Migrator.DocumentTypes.Components.Builders

{
    public class MacroComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "macro";

        public MacroComponentBuilder(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper, IDataTypeService dataTypeService) : base(
            contentTypeService, shortStringHelper, dataTypeService)
        {
            Alias = "macro";
            Name = "Macro";
        }

        public override bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public override void Build()
        {
            AddProperty("macro", "Macro", Constants.DataTypes.Textarea);
            Save();
        }

        public override object MapData(EnterspeedPropertyType enterspeedProperty)
        {
            var data = new Dictionary<string, object>();
            var macro = GetValue(enterspeedProperty, "value").ToString();
            data.Add("macro", macro);

            return data;
        }
    }
}