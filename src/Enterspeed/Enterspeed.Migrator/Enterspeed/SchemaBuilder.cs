using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Enterspeed.Contracts;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Enterspeed
{
    public class SchemaBuilder : ISchemaBuilder
    {
        public Schemas BuildPageSchemas(List<PageData> pageDatas)
        {
            var schemas = new Schemas();
            BuildPageSchemas(pageDatas, schemas);
            return schemas;
        }

        public void BuildPageSchemas(List<PageData> pageDatas, Schemas schemas)
        {
            foreach (var pageData in pageDatas)
            {
                var alias = pageData.MetaSchema.SourceEntityAlias;
                var page = schemas.Pages.FirstOrDefault(p => p.MetaSchema.SourceEntityAlias == alias);
                var properties = pageData.Properties;

                if (page == null)
                {
                    schemas.Pages.Add(new Schema
                    {
                        MetaSchema = pageData.MetaSchema,
                        Properties = properties.ToList()
                    });
                }
                else
                {
                    foreach (var property in properties)
                    {
                        var existingProperty = pageData.Properties.FirstOrDefault(p => p.Alias == property.Alias);
                        if (existingProperty == null)
                        {
                            page.Properties.Add(property);
                        }
                    }
                }

                if (pageData.Children != null && pageData.Children.Any())
                {
                    BuildPageSchemas(pageData.Children, schemas);
                }
            }
        }
    }
}