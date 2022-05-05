using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Migrator.Models;

namespace Umbraco9.Migrator.Umbraco.Contracts
{
    public interface IDocumentTypeBuilder
    {
        Task<IEnumerable<UmbracoDoctype>> BuildDoctypesAsync(List<DocumentTypes> schemas);
    }
}