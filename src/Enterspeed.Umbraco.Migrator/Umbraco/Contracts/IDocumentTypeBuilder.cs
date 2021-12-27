using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Umbraco.Migrator.Models;

namespace Enterspeed.Umbraco.Migrator.Umbraco.Contracts
{
    public interface IDocumentTypeBuilder
    {
        Task<IEnumerable<UmbracoDoctype>> BuildDoctypesAsync(List<Schema> schemas);
    }
}