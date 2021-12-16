using Enterspeed.Umbraco.Migrator;
using Enterspeed.Umbraco.Migrator.Settings;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Umbraco91.Migrator
{
    public class MigratorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();
            builder.Services.AddSingleton(new EnterspeedConfiguration
            {
                ApiKey = builder.Config.GetSection("Enterspeeed").GetValue<string>("ApiKey"),
                Handles = builder.Config.GetSection("Handles").GetValue<List<string>>("Handles")
            });
        }
    }
}
