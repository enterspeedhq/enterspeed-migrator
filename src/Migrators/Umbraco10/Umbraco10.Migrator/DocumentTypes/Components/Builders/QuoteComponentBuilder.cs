using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Umbraco10.Migrator.DocumentTypes.Components.Builders
{
    public class QuoteComponentBuilder : ComponentBuilder
    {
        private const string PropertyAlias = "quote";

        public QuoteComponentBuilder(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper, IDataTypeService dataTypeService)
            : base(contentTypeService, shortStringHelper, dataTypeService)
        {
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
    }
}