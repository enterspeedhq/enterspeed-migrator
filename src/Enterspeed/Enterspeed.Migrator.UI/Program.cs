using Enterspeed.Migrator;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((c, services) =>
    {
        services.RegisterEnterspeedServices(c.Configuration);
    }).Build();


var schemaImporter = host.Services.GetService<ISchemaImporter>();
if (schemaImporter != null)
{
    Console.WriteLine("Importing schemas");
    var schemas = await schemaImporter.ImportSchemasAsync();
    Console.ReadLine();
}
