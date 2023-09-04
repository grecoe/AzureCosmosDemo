namespace CosmosCoreUtil.Attributes
{
    public class CosmosEntityAttribute : Attribute
    {
        public string Database { get; set; }
        public string Collection { get; set; }

        public CosmosEntityAttribute(string database, string collection)
        {
            this.Database = database;
            this.Collection = collection;
        }
    }
}
