namespace Enterspeed.Umbraco.Migrator.ValueTypes
{
    public class UmbracoSection : IPropertyType
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}