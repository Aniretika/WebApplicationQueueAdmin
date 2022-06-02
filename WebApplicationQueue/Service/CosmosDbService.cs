using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplicationQueue.Service
{
    public class CosmosDbService : ICosmosDbService
    {
        protected Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<T> GetItem<T>(Guid id, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()), null, cancellationToken);
                return response.Resource;
            }
            catch (CosmosException cosmoException) when (cosmoException.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
        }

        public async Task<List<T>> GetItems<T>(string queryString, CancellationToken cancellationToken) where T : class
        {
            try
            {
                var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
                var LResults = new List<T>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync(cancellationToken);
                    LResults.AddRange(response);
                }

                return LResults;
            }
            catch (CosmosException cosmoException) when (cosmoException.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
        }

        public async Task<HttpStatusCode> AddItem<T>(Guid id, T item, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _container.CreateItemAsync(item, new PartitionKey(id.ToString()), null, cancellationToken);
                return response.StatusCode;
            }
            catch (CosmosException cosmoException) when (cosmoException.StatusCode != HttpStatusCode.Created)
            {
                return cosmoException.StatusCode;
            }
        }

        public async Task<HttpStatusCode> UpsertItem<T>(Guid id, T item, CancellationToken cancellationToken = default) 
        {
            try
            {
                var response = await _container.UpsertItemAsync(item, new PartitionKey(id.ToString()), null, cancellationToken);
               
                return response.StatusCode;
            }
            catch (CosmosException cosmoException) when (cosmoException.StatusCode != HttpStatusCode.OK)
            {
                return cosmoException.StatusCode;
            }
        }
        
        public async Task<HttpStatusCode> DeleteItem<T>(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()), null, cancellationToken);
                return response.StatusCode;
            }
            catch (CosmosException cosmoException) when (cosmoException.StatusCode != HttpStatusCode.NoContent)
            {
                return cosmoException.StatusCode;
            }
        }

        public async Task<HttpStatusCode> UpdateItem<T>(Guid id, T item, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _container.ReplaceItemAsync(item, id.ToString(), null, null, cancellationToken);

                return response.StatusCode;
            }
            catch (CosmosException cosmoException) when (cosmoException.StatusCode != HttpStatusCode.OK)
            {
                return cosmoException.StatusCode;
            }
        }
    }
}
