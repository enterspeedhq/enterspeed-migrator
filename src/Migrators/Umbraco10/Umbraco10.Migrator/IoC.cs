using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco10.Migrator.Content;
using Umbraco10.Migrator.DocumentTypes;
using Umbraco10.Migrator.DocumentTypes.Components.Builders;
using Umbraco10.Migrator.Settings;

namespace Umbraco10.Migrator
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
            serviceCollection.AddTransient<IComponentBuilder, EmbedComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, HeadlineComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, MacroComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, MediaComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, QuoteComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, RTEComponentBuilder>();
        }
    }
}