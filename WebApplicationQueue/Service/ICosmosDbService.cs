using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplicationQueue.Service
{
    public interface ICosmosDbService
    {
        Task<T> GetItem<T>(Guid id, CancellationToken cancellationToken) where T : class;

        Task<List<T>> GetItems<T>(string AQueryString, CancellationToken cancellationToken) where T : class;

        Task<HttpStatusCode> AddItem<T>(Guid id, T item, CancellationToken cancellationToken);

        Task<HttpStatusCode> UpsertItem<T>(Guid id, T item, CancellationToken cancellationToken);

        Task<HttpStatusCode> DeleteItem<T>(Guid id, CancellationToken cancellationToken);

        Task<HttpStatusCode> UpdateItem<T>(Guid id, T item, CancellationToken cancellationToken);
    }
}