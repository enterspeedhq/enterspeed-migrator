namespace Umbraco9.Migrator.Settings
{
    public class UmbracoMigrationConfiguration
    {
        public static string ConfigurationKey => "UmbracoMigrationConfiguration";
        public string RootDocType { get; set; }
        public string ContentPropertyAlias { get; set; }
    }
}