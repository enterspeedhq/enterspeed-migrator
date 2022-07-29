using System.Collections.Generic;
using System.Text.Json;
using Enterspeed.Migrator.ValueTypes;

namespace Enterspeed.Migrator.Enterspeed.Contracts
{
    public interface IPropertyResolver
    {
        IPropertyType Resolve(JsonProperty jsonProperty);
    }
}