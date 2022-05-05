using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Contracts
{
    public interface IDocumentTypeBuilder
    {
        Task<IEnumerable<UmbracoDoctype>> BuildDoctypesAsync(List<DocumentTypes> schemas);
    }
}