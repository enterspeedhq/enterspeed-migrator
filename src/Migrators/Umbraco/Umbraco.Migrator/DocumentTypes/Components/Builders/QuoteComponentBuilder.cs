using Enterspeed.Migrator.ValueTypes;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Migrator.DocumentTypes.Components.Builders
{
    public class QuoteComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "quote";

        public QuoteComponentBuilder(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper, IDataTypeService dataTypeService)
            : base(contentTypeService, shortStringHelper, dataTypeService)
        {
            Alias = "quote";
            Name = "Quote";
        }

        public override bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public override void Build()
        {
            AddProperty("quote", "Quote", Constants.DataTypes.Textarea);
            Save();
        }

        public override object MapData(EnterspeedPropertyType enterspeedProperty)
        {
            var data = new Dictionary<string, object>();
            var quote = GetValue(enterspeedProperty, "value").ToString();
            data.Add("quote", quote);

            return data;
        }
    }
}