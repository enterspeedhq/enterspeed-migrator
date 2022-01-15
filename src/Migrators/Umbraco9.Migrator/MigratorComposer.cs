using System.Collections.Generic;   
using Enterspeed.Umbraco.Migrator;
using Enterspeed.Umbraco.Migrator.Settings;
using Enterspeed.Umbraco.Migrator.Umbraco.Contracts;
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
            var enterspeedConfiguration = new EnterspeedConfiguration
            {
                ApiKey = builder.Config.GetSection("Enterspeed").GetValue<string>("ApiKey"),
                NavigationHandles = builder.Config.GetSection("Enterspeed:NavigationHandles").Get<List<NavigationHandle>>()
            };

            builder.Services.AddSingleton(enterspeedConfiguration);
            builder.Services.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();
            builder.Services.RegisterEnterspeedServices();
        }
    }
}