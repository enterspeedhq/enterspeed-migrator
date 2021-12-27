using System.Collections.Generic;
using Enterspeed.Umbraco.Migrator;
using Enterspeed.Umbraco.Migrator.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco9.Migrator
{
    public class MigratorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();

            var enterspeedSection = builder.Config.GetSection("Enterspeeed");
            builder.Services.AddSingleton(new EnterspeedConfiguration
            {
                ApiKey = enterspeedSection.GetValue<string>("ApiKey"),
                NavigationHandles = enterspeedSection.GetValue<IDictionary<string, string>>("NavigationHandles")
            });
        }
    }
}