using Enterspeed.Migrator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Enterspeed.Migrator.Settings;
using Umbraco9.Migrator.Umbraco.Contracts;
using Umbraco9.Migrator.Umbraco;

namespace Umbraco9.Migrator
{
    public class MigratorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var enterspeedSection = builder.Config.GetSection("Enterspeed");
            var enterspeedConfiguration = new EnterspeedConfiguration
            {
                ApiKey = enterspeedSection.GetValue<string>("ApiKey"),
                NavigationHandle = enterspeedSection.GetValue<string>("NavigationHandle")
            };

            builder.Services.AddSingleton(enterspeedConfiguration);
            builder.Services.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();
            builder.Services.AddTransient<IContentBuilder, ContentBuilder>();
            builder.Services.AddTransient<IDocumentTypeBuilder, DocumentTypeBuilder>();
            builder.Services.RegisterEnterspeedServices();
        }
    }
}