using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationQueue.Entities;
using WebApplicationQueue.Service;

namespace WebApplicationQueue.Application.Queries.GetList
{
    public class MainQueue
    {
        public class Query : IRequest<List<Position>>
        {
        }

        public class QueueHandler : IRequestHandler<Query, List<Position>>
        {
            private readonly ICosmosDbService _cosmosDbService;

            public QueueHandler(ICosmosDbService CosmosDbService)
                => _cosmosDbService = CosmosDbService;

            public async Task<List<Position>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _cosmosDbService.GetItems<Position>($"select * from {nameof(Position)}", cancellationToken);
            }
        }
    }
}
