namespace Enterspeed.Migrator.ValueTypes
{
    public class UmbracoTextArea : IPropertyType
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}