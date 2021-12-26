namespace Enterspeed.Umbraco.Migrator.Enterspeed.Contracts
{
    public interface ISourceImporter
    {
        Task<List<object>> ImportAllDataSourcesAsync(Dictionary<string, object> views);
    }
}