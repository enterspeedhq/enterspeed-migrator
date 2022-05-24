namespace Enterspeed.Migrator.Models
{
    public class EntityTypeMeta
    {
        public EntityTypeMeta()
        {

        }

        public EntityTypeMeta(string sourceEntityAlias, string sourceEntityName)
        {
            SourceEntityAlias = sourceEntityAlias;
            SourceEntityName = sourceEntityName;
        }

        public string SourceEntityName { get; set; }
        public string SourceEntityAlias { get; set; }
    }
}
