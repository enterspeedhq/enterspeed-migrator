using Enterspeed.Migrator.ValueTypes;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Migrator.DocumentTypes.Components.Builders

{
    public class RTEComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "rte";

        public RTEComponentBuilder(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper, IDataTypeService dataTypeService)
            : base(contentTypeService, shortStringHelper, dataTypeService)
        {
            Alias = "rte";
            Name = "RTE";
        }

        public override bool CanBuild(string propertyAlias)
        {
            return PropertyAlias.Equals(propertyAlias);
        }

        public override void Build()
        {
            AddProperty("rte", "RTE", Constants.DataTypes.RichtextEditor);
            Save();
        }

        public override object MapData(EnterspeedPropertyType enterspeedProperty)
        {
            var data = new Dictionary<string, object>();
            var rte = GetValue(enterspeedProperty, "value").ToString();
            data.Add("rte", rte);

            return data;
        }
    }
}