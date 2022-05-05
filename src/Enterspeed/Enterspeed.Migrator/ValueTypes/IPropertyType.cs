namespace Enterspeed.Migrator.ValueTypes
{
    public interface IPropertyType
    {
        string Name { get; set; }
        string Alias { get; set; }
        string Type { get; set; }
        string Value { get; set; }
    }
}