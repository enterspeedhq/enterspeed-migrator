using System.Collections.Generic;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface ISchemaBuilder
    {
        Schemas BuildPageSchemas(List<PageData> pageData);
    }
}