using Enterspeed.Migrator.ValueTypes;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Migrator.DocumentTypes.Components.Builders

{
    public class EmbedComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "embed";

        public EmbedComponentBuilder(IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService)
            : base(contentTypeService, shortStringHelper, dataTypeService)
        {
            Alias = "embed";
            Name = "Embed";
        }

        public override bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public override void Build()
        {
            AddProperty("embed", "Embed", Constants.DataTypes.Textarea);
            Save();
        }
        public override object MapData(EnterspeedPropertyType enterspeedProperty)
        {
            var data = new Dictionary<string, object>();
            var embed = GetValue(enterspeedProperty, "value").ToString();
            data.Add("embed", embed);

            return data;
        }
    }
}