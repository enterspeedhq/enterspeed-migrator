using Enterspeed.Migrator;
using Enterspeed.Migrator.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Enterspeed.Migrator.Settings;

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
            builder.Services.RegisterEnterspeedServices();
        }
    }
}