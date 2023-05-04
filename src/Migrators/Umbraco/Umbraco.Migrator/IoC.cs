using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Migrator.Content;
using Umbraco.Migrator.DocumentTypes;
using Umbraco.Migrator.DocumentTypes.Components;
using Umbraco.Migrator.DocumentTypes.Components.Builders;
using Umbraco.Migrator.Settings;

namespace Umbraco.Migrator
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

            serviceCollection.AddTransient<IComponentBuilderHandler, ComponentBuilderHandler>();
            serviceCollection.AddTransient<IComponentBuilder, EmbedComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, HeadlineComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, MacroComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, MediaComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, QuoteComponentBuilder>();
            serviceCollection.AddTransient<IComponentBuilder, RTEComponentBuilder>();
        }
    }
}