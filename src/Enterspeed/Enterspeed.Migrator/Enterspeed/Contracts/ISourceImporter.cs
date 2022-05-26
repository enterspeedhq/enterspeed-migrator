using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface ISourceImporter
    {
        Task<List<PageEntityType>> ImportDataAsync();
    }
}