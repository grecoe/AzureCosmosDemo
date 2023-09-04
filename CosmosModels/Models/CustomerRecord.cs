namespace CosmosModels.Models
{
    using CosmosCoreUtil.Attributes;
    using CosmosCoreUtil.Definitions;
    using CosmosCoreUtil.Domain;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;
    using System.Linq.Expressions;

    [CosmosEntityAttribute(database: "Customers", collection: "Customer")]
    public class CustomerRecord : BaseCosmosRecord
    {
        [JsonProperty(PropertyName = "id")]
        public override string Id { get; set; } = (new Guid()).ToString();

        // Override the ID and PartitionKey
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = string.Empty;

        // Override the ID and PartitionKey
        [JsonProperty(PropertyName = "address1")]
        public string Address1{ get; set; } = string.Empty;


        /// <summary>
        /// Search using a query.
        /// </summary>
        public static async Task<CustomerRecord?> FindCustomerByName(string name, ICosmosConnection connection)
        {
            string query = $"SELECT * from c where c.name = '{name}'";
            QueryDefinition qd = new QueryDefinition(query);

            var results =  await connection.QueryItemsAsync<CustomerRecord>(qd);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Search using a query.
        /// </summary>
        public static async Task<IList<CustomerRecord>> FindCustomerByAddress(string address1, ICosmosConnection connection)
        {
            Expression<Func<CustomerRecord, bool>> predicateOnAddress =
                    b => b.Address1 == address1;

            return (await connection.ReadItemsAcrossPartitionsAsync<CustomerRecord>(predicateOnAddress)).ToList();
        }
    }
}
