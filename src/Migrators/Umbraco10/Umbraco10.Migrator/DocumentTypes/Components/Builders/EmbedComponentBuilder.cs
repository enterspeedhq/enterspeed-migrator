using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco10.Migrator.DocumentTypes.Components.Builders

{
    public class EmbedComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "embed";

        public EmbedComponentBuilder(IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            IDataTypeService dataTypeService)
            : base(contentTypeService, shortStringHelper, dataTypeService)
        {
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
    }
}