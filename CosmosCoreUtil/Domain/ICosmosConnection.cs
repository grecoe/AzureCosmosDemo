
namespace CosmosCoreUtil.Domain
{
    using CosmosCoreUtil.Definitions;
    using Microsoft.Azure.Cosmos;
    using System.Linq.Expressions;
    using Logging.Local;

    public interface ICosmosConnection
    {
        public ILogger Logger { get; }
        public CosmosConfiguration CosmosConfiguration { get; }
        public CosmosClient Client { get; }

        public Task<IList<T>> QueryItemsAsync<T>(QueryDefinition? query = null)
            where T : BaseCosmosRecord, new();
        public Task<IList<T>> ReadItemsAcrossPartitionsAsync<T>(Expression<Func<T, bool>> predicate)
            where T : BaseCosmosRecord, new();
        public Task<string> UpsertSerializedItemAsync<T>(T entity)
            where T : BaseCosmosRecord, new();
        public Task<int> DeleteContainerRecords<T>()
            where T : BaseCosmosRecord, new();
        public Task<bool> DeleteRecord<T>(T record)
            where T : BaseCosmosRecord, new();
    }
}
