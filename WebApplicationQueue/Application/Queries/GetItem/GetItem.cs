using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationQueue.Entities;
using WebApplicationQueue.Service;

namespace WebApplicationQueue.Application.Queries.GetItem
{
    public class GetItem
    {
        public class Query : IRequest<Position>
        {
            public Guid Id { get; set; }
        }
        public class GetItemHandler : IRequestHandler<Query, Position>
        {
            private readonly ICosmosDbService _cosmosDbService;

            public GetItemHandler(ICosmosDbService ACosmosDbService)
                => _cosmosDbService = ACosmosDbService;

            public async Task<Position> Handle(Query request, CancellationToken ACancellationToken)
            {
                return await _cosmosDbService.GetItem<Position>(request.Id, ACancellationToken);
            }
        }
    }
}
