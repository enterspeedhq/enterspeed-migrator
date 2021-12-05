using Enterspeed.Umbraco.Migrator;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco91.Migrator
{
    public class MigratorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IUmbracoMigratorService, UmbracoMigratorService>();
        }
    }
}
