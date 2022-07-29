using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaBuilder : ISchemaBuilder
    {
        public void BuildPages(Schemas schemas, List<PageData> pageData)
        {
            var pages = new List<Schema>();
            foreach (var data in pageData)
            {
                var alias = data.MetaSchema.SourceEntityAlias;
                var page = pages.FirstOrDefault(p => p.MetaSchema.SourceEntityAlias == alias);
                var properties = data.Properties.DistinctBy(p => p.Alias);

                if (page == null)
                {
                    pages.Add(new Schema()
                    {
                        MetaSchema = data.MetaSchema,
                        Properties = properties.ToList()
                    });
                }
                else
                {
                    foreach (var property in properties)
                    {
                        var existingProperty = data.Properties.FirstOrDefault(p => p.Alias == property.Alias);
                        if (existingProperty == null)
                        {
                            page.Properties.Add(property);
                        }
                    }
                }
            }
        }

        public void BuildComponents()
        {
        }
    }
}