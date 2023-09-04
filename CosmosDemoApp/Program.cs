namespace CosmosDemoApp
{
    using CosmosCoreUtil.Domain;
    using CosmosModels.Models;
    using Logging.Local;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            // You can set this up multiple ways
            // 1. Endpoint is complete with key 
            // 2. Endpoint and Key have values (think KeyVault)
            // 3. Endpoint and ResourceId in which the local Azure Credential looks up a key.
            LocalLogger logger = new LocalLogger();
            CosmosConfiguration config = new CosmosConfiguration()
            {
                Endpoint = "https://[YOUR_COSMOS_NAME].documents.azure.com:443/",
                ResourceId = "/subscriptions/[YOUR_SUB_ID]/resourceGroups/[YOUR_RG]/providers/Microsoft.DocumentDb/databaseAccounts/[YOUR_COSMOS_NAME]"
            };
            CosmosConnection conn = new CosmosConnection(config, logger);


            // Create two istances
            string[] customers = new string[]
            {
                "Steve",
                "Larry"
            };

            logger.LogInformation("Creating Customer Records");
            foreach (string customer in customers)
            {
                CustomerRecord exampleRecord = new CustomerRecord()
                {
                    Name = customer,
                    Address1 = "1 Main Street",
                    PartitionKey = "newCustomers"
                };
                exampleRecord.Id = Guid.NewGuid().ToString();

                await conn.UpsertSerializedItemAsync(exampleRecord);
            }

            // Search, get all records, delete records check (CRUD)
            logger.LogInformation("Retrieving record for Steve");
            CustomerRecord? steve = await CustomerRecord.FindCustomerByName("Steve", conn);
            if(steve == null)
            {
                throw new Exception("Steve");
            }

            logger.LogInformation("Retrieving Customers at an address");
            IList<CustomerRecord> addressSearch = await CustomerRecord.FindCustomerByAddress("1 Main Street", conn);
            if (addressSearch.Count != 2)
            {
                throw new Exception("Count");
            }

            logger.LogInformation("Get all collection records.");
            IList<CustomerRecord> records = await conn.QueryItemsAsync<CustomerRecord>();
            if(records.Count != 2)
            {
                throw new Exception("Count");
            }

            logger.LogInformation("Delete all collection records.");
            int deleteCount = await conn.DeleteContainerRecords<CustomerRecord>();
            if (deleteCount!= 2)
            {
                throw new Exception("Delete Count");
            }

            logger.LogInformation("Done....");
        }
    }
}