using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Delivery.Sdk.Configuration;
using Enterspeed.Delivery.Sdk.Domain.Connection;
using Enterspeed.Delivery.Sdk.Domain.Providers;
using Enterspeed.Delivery.Sdk.Domain.Services;
using Enterspeed.Delivery.Sdk.Domain.SystemTextJson;
using Enterspeed.Umbraco.Migrator.Enterspeed;
using Enterspeed.Umbraco.Migrator.Enterspeed.Contracts;
using Enterspeed.Umbraco.Migrator.Umbraco;
using Enterspeed.Umbraco.Migrator.Umbraco.Contracts;
using Microsoft.Extensions.DependencyInjection;


namespace Enterspeed.Umbraco.Migrator
{
    public static class IoC
    {
        public static void RegisterEnterspeedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IContentBuilder, ContentBuilder>();
            serviceCollection.AddTransient<IDocumentTypeBuilder, DocumentTypeBuilder>();
            serviceCollection.AddTransient<IApiService, ApiService>();
            serviceCollection.AddTransient<ISchemaImporter, SchemaImporter>();
            serviceCollection.AddTransient<ISourceImporter, SourceImporter>();
            serviceCollection.AddTransient<IEnterspeedDeliveryService, EnterspeedDeliveryService>();
            serviceCollection.AddTransient<IEnterspeedConfigurationProvider, InMemoryConfigurationProvider>();
            serviceCollection.AddTransient<EnterspeedDeliveryConnection>();
            serviceCollection.AddSingleton(new EnterspeedDeliveryConfiguration());
            serviceCollection.AddTransient<IJsonSerializer, SystemTextJsonSerializer>();
        }
    }
}