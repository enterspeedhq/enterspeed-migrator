using Microsoft.Extensions.DependencyInjection;
using Umbraco9.Migrator.Umbraco.Contracts;
using Umbraco9.Migrator.Builders;
using Umbraco9.Migrator.Builders.Contracts;

namespace Umbraco9.Migrator
{
    public static class IoC
    {
        public static void RegisterUmbraco9MigratorService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();
            serviceCollection.AddTransient<IContentBuilder, ContentBuilder>();
            serviceCollection.AddTransient<IDocumentTypeBuilder, DocumentTypeBuilder>();
        }
    }
}