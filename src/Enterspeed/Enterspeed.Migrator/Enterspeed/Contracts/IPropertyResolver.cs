using System.Collections.Generic;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IPropertyResolver
    {
        IPropertyType Resolve(string key, Dictionary<string, object> view);
    }
}