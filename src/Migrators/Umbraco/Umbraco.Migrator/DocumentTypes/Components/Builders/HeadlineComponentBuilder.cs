using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Migrator.DocumentTypes.Components.Builders

{
    public class HeadlineComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "headline";

        public HeadlineComponentBuilder(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper, IDataTypeService dataTypeService) : base(
            contentTypeService, shortStringHelper, dataTypeService)
        {
            Alias = "headline";
            Name = "Headline";
        }

        public override bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public override void Build()
        {
            AddProperty("headline", "Headline", Constants.DataTypes.Textbox);
            Save();
        }

        public override object MapData(EnterspeedPropertyType enterspeedProperty)
        {
            var data = new Dictionary<string, object>();
            var headline = GetValue(enterspeedProperty, "value").ToString();
            data.Add("headline", headline);

            return data;
        }
    }
}