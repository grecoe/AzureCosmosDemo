namespace CosmosCoreUtil.Domain
{
    using CosmosCoreUtil.Attributes;
    using CosmosCoreUtil.Definitions;
    using CosmosCoreUtil.Domain.Utils;
    using Logging.Local;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    public class CosmosSelection
    {
        public string Database { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public Container? Container { get; set; }
    }

    public class CosmosConnection : ICosmosConnection
    {
        /// <summary>
        /// NOTE: There is no real explicit logging to be done here as we will reuse
        /// the COSMOS code from the OEP-RP Resource Provider, but for debugging purposes
        /// some may be added.
        /// </summary>
        public ILogger Logger { get; private set; }
        public CosmosConfiguration CosmosConfiguration { get; private set; }
        public CosmosClient Client { get; private set; }
        private List<CosmosSelection> Options { get; set; } = new List<CosmosSelection>();

#pragma warning disable CS8618
        public CosmosConnection(CosmosConfiguration configuration, ILogger logger)
        {
            this.CosmosConfiguration = configuration;
            this.Logger = logger;

            // TODO: Look in here to find out where to acquire a key.
            this.GetCosmosClient().Wait();
        }
#pragma warning restore CS8618

        public async Task<IList<T>> QueryItemsAsync<T>(QueryDefinition? query = null)
            where T : BaseCosmosRecord, new()
        {
            CosmosSelection? selection = this.GetContainer<T>();
            if (selection == null || selection.Container == null)
            {
                Logger.LogError($"Unable to determine container for {typeof(T).Name}");
                throw new Exception("Unable to determine container");
            }

            if (query == null)
            {
                query = new QueryDefinition(query: $"SELECT * FROM {selection.ContainerName}");
            }

            IList<T> returnValue = new List<T>();
            try
            {
                returnValue = await CosmosConnection.QueryItemsAsync<T>(selection.Container, query);
            }
            catch(Exception ex)
            {
                Logger.LogException(ex, "QueryItemsAsync");
            }

            return returnValue;
        }

        public async Task<IList<T>> ReadItemsAcrossPartitionsAsync<T>(Expression<Func<T, bool>> predicate)
            where T : BaseCosmosRecord, new()
        {
            CosmosSelection? selection = this.GetContainer<T>();
            if (selection == null || selection.Container == null)
            {
                Logger.LogError($"Unable to determine container for {typeof(T).Name}");
                throw new Exception("Unable to determine container");
            }

            IList<T> returnValue = new List<T>();
            try
            {
                returnValue = await CosmosConnection.ReadItemsAcrossPartitionsAsync(selection.Container, predicate);
            }
            catch(Exception ex)
            {
                Logger.LogException(ex, "ReadItemsAcrossPartitionsAsync");

            }
            return returnValue;
        }

        public async Task<string> UpsertSerializedItemAsync<T>(T entity)
            where T : BaseCosmosRecord, new()
        {
            CosmosSelection? selection = this.GetContainer<T>();
            if (selection == null || selection.Container == null)
            {
                throw new Exception("Unable to determine container");
            }

            entity.LastModifiedTime = DateTime.UtcNow;

            var entity_str = JsonConvert.SerializeObject(entity, Formatting.Indented);
            Microsoft.Azure.Cosmos.PartitionKey key = new Microsoft.Azure.Cosmos.PartitionKey(entity.PartitionKey);

            string response = string.Empty;

            try
            {
                response = await CosmosConnection.UpsertSerializedItemAsync(selection.Container, entity_str, key);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"Failed to upsert item of type {typeof(T).Name}, retry in progress.");
                await Task.Delay(500);
                response = await CosmosConnection.UpsertSerializedItemAsync(selection.Container, entity_str, key);
            }

            return response;
        }

        public async Task<int> DeleteContainerRecords<T>()
            where T : BaseCosmosRecord, new()
        {
            int deleted_records = 0;

            //throw new Exception("Do you really know what you are doing?");
            CosmosSelection? selected_container = this.GetContainer<T>();
            if (selected_container == null || selected_container == null)
            {
                Logger.LogError($"Unable to determine container for {typeof(T).Name}");
                throw new Exception("Cannot get container for this type");
            }

            IList<T> containerEntries = await this.QueryItemsAsync<T>();

            foreach (T entity in containerEntries)
            {
#pragma warning disable CS8602
                PartitionKey pk = new PartitionKey(entity.PartitionKey);
                var response = await selected_container.Container.DeleteItemAsync<T>(entity.Id, pk, null);
                deleted_records += 1;
#pragma warning restore CS8602
            }
            return deleted_records;
        }

        public async Task<bool> DeleteRecord<T>(T record)
            where T : BaseCosmosRecord, new()
        {
            CosmosSelection? selected_container = this.GetContainer<T>();
            if (selected_container == null || selected_container == null)
            {
                Logger.LogError($"Unable to determine container for {typeof(T).Name}");
                throw new Exception("Cannot get container for this type");
            }

            PartitionKey pk = new PartitionKey(record.PartitionKey);
#pragma warning disable CS8602
            var response = await selected_container.Container.DeleteItemAsync<T>(record.Id, pk, null);
#pragma warning restore CS8602

            if (response != null)
            {
                return true;
            }
            return false;
        }

        private async Task GetCosmosClient()
        {
            if (string.IsNullOrEmpty(this.CosmosConfiguration.Endpoint))
            {
                Logger.LogError("Configuration Endpoint cannot be empty");
                throw new ArgumentException("Cannot generate CosmosClient with no Cosmos Endpoint");
            }

            if (!string.IsNullOrEmpty(this.CosmosConfiguration.Key))
            {
                Logger.LogInformation("Creating CosmosClient with Endpoint and Key");
                this.Client = new CosmosClient(this.CosmosConfiguration.Endpoint, this.CosmosConfiguration.Key);
            }
            else if (this.CosmosConfiguration.Endpoint.ToLower().Contains("accountkey="))
            {
                Logger.LogInformation("Creating CosmosClient with Endpoint containing key");
                this.Client = new CosmosClient(this.CosmosConfiguration.Endpoint);
            }
            else if (!string.IsNullOrEmpty(this.CosmosConfiguration.ResourceId))
            {
                Logger.LogInformation("Creating CosmosClient with Endpoint and ResourceId");
                // We have no key and there is not one in the configuration, we have to acquire one so this
                // exception serves as a TODO.
                string access_key = await AzureResourceKeyFetcher.GetCosmosDbPrimaryMasterKeyAsync(
                    this.CosmosConfiguration.ResourceId);
                this.Client = new CosmosClient(this.CosmosConfiguration.Endpoint, access_key);
            }
            else
            {
                Logger.LogError("Cannot generate CosmosClient with no Cosmos Key");
                throw new ArgumentException("Cannot generate CosmosClient with no Cosmos Key");
            }
        }
       
        private CosmosSelection? GetContainer<T>()
            where T : BaseCosmosRecord, new()
        {
            Type genericType = typeof(T);
            // TODO: Fix this 
            Type cosmosType = typeof(T);
            object[] classAttrs = cosmosType.GetCustomAttributes(true);
            var cust = classAttrs.Where(x => (x as CosmosEntityAttribute) != null).ToList();

            if(!cust.Any())
            {
                Logger.LogError($"Cannot find CosmosEntityAttribute on object {cosmosType.Name}");
                throw new Exception($"Cannot find CosmosEntityAttribute on object {cosmosType.Name}");
            }

            CosmosEntityAttribute cosmosEntityAttribute = cust.First() as CosmosEntityAttribute;

            // Have we seen it yet?
            List<CosmosSelection> selections = 
                this.Options.Where(x => x.Database == cosmosEntityAttribute.Database &&
                    x.ContainerName == cosmosEntityAttribute.Collection)
                .ToList();

            if(!selections.Any())
            {
                Logger.LogInformation($"Creating Container for {cosmosEntityAttribute.Database} : {cosmosEntityAttribute.Collection}");

                CosmosSelection newSelection = new CosmosSelection()
                {
                    Database = cosmosEntityAttribute.Database,
                    ContainerName = cosmosEntityAttribute.Collection,
                    Container = this.Client.GetContainer(cosmosEntityAttribute.Database, cosmosEntityAttribute.Collection)
                };

                this.Options.Add(newSelection);
                selections.Add(newSelection);
            }
            else
            {
                Logger.LogInformation($"Found cached Container for {cosmosEntityAttribute.Database} : {cosmosEntityAttribute.Collection}");
            }
            return selections.FirstOrDefault();
        }

        private static async Task<IList<T>> QueryItemsAsync<T>(Container container, QueryDefinition querydefinition)
        {
            FeedIterator<T> queryResultSetIterator = container.GetItemQueryIterator<T>(querydefinition);
            List<T> results = new List<T>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                results.AddRange(currentResultSet);
            }

            return results;
        }

        private static async Task<List<T>> ReadItemsAcrossPartitionsAsync<T>(Container container, Expression<Func<T, bool>> predicate)
            where T : BaseCosmosRecord
        {
            var feedIterator = container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true)
                .Where(predicate)
                .ToFeedIterator();

            var results = new List<T>();

            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                results.AddRange(response.Resource);
            }

            return results;
        }

        private static async Task<string> UpsertSerializedItemAsync(
            Container container,
            string entity,
            PartitionKey partitionKey,
            ItemRequestOptions? requestOptions = null)
        {
            string return_value = string.Empty;

            try
            {
                var stream = ToStream(entity);
                var responseMessage = await container.UpsertItemStreamAsync(stream, partitionKey, requestOptions);
                responseMessage.EnsureSuccessStatusCode();
                return_value = FromStream(responseMessage.Content);
            }
            catch (Exception ex)
            {
                Exception ex_extended = new Exception(container.Id, ex);
                throw ex_extended;
            }
            return return_value;
        }

        private static Stream ToStream(string input)
        {
            var byteArray = Encoding.ASCII.GetBytes(input);
            return new MemoryStream(byteArray);
        }

        private static string FromStream(Stream input)
        {
            var reader = new StreamReader(input);
            return reader.ReadToEnd();
        }
    }
}
