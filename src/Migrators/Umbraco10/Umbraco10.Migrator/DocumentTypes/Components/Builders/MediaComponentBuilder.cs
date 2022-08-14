using Enterspeed.Migrator.ValueTypes;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco10.Migrator.DocumentTypes.Components.Builders
{
    public class MediaComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "media";

        public MediaComponentBuilder(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper, IDataTypeService dataTypeService) : base(
            contentTypeService, shortStringHelper, dataTypeService)
        {
            Alias = "media";
            Name = "Media";
        }

        public override bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public override void Build()
        {
            AddProperty("media", "Media", Constants.DataTypes.Textarea);
            Save();
        }

        public override object MapData(EnterspeedPropertyType enterspeedProperty)
        {
            var data = new Dictionary<string, object>();
            var media = GetValue(enterspeedProperty, "value");
            data.Add("media", media);

            return data;
        }
    }
}