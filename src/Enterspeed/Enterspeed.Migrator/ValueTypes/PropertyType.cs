namespace Enterspeed.Migrator.ValueTypes
{
    public class PropertyType : IPropertyType
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public string Source { get; set; }
    }
}
