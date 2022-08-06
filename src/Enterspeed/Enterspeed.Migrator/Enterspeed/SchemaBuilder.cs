using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaBuilder : ISchemaBuilder
    {
        public Schemas BuildPageSchemas(List<PageData> pageData)
        {
            var schemas = new Schemas();
            BuildPageSchemas(pageData, schemas);
            return schemas;
        }

        public void BuildPageSchemas(List<PageData> pageData, Schemas schemas)
        {
            foreach (var data in pageData)
            {
                var alias = data.MetaSchema.SourceEntityAlias;
                var page = schemas.Pages.FirstOrDefault(p => p.MetaSchema.SourceEntityAlias == alias);
                var properties = data.Properties.Where(p => !string.IsNullOrEmpty(p.Value.ToString()));

                if (page == null)
                {
                    schemas.Pages.Add(new Schema
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

                if (data.Children != null && data.Children.Any())
                {
                    BuildPageSchemas(data.Children, schemas);
                }
            }
        }
    }
}