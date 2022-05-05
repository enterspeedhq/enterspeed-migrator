using Enterspeed.Migrator;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.RegisterEnterspeedServices())
    .Build();

