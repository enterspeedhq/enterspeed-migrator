using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco9.Migrator.Builders;
using Umbraco9.Migrator.Builders.Contracts;
using Umbraco9.Migrator.Settings;

namespace Umbraco9.Migrator
{
    public static class IoC
    {
        public static void RegisterUmbraco9MigratorService(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();
            serviceCollection.AddTransient<IContentBuilder, ContentBuilder>();
            serviceCollection.AddTransient<IDocumentTypeBuilder, DocumentTypeBuilder>();
            serviceCollection.Configure<UmbracoMigrationConfiguration>(setting =>
                configuration.GetSection(UmbracoMigrationConfiguration.ConfigurationKey).Bind(setting));
        }
    }
}