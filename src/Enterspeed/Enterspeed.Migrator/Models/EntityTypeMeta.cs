namespace Enterspeed.Migrator.Models
{
    public class EntityTypeMeta
    {
        public EntityTypeMeta()
        {

        }

        public EntityTypeMeta(string sourceEntityAlias, string sourceEntityName, string contentName)
        {
            SourceEntityAlias = sourceEntityAlias;
            SourceEntityName = sourceEntityName;
            ContentName = contentName;
        }

        public string SourceEntityName { get; set; }
        public string SourceEntityAlias { get; set; }
        public string ContentName { get; set; }
    }
}
