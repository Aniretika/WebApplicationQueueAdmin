using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationQueue.Application.Core;
using WebApplicationQueue.Entities;
using WebApplicationQueue.Service;
using Unit = MediatR.Unit;

namespace WebApplicationQueue.Application.Commands.Delete
{
    public class DeletePosition
    {
        public class CommandDelete : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        public class DeletePositionHandler : IRequestHandler<CommandDelete, Result<Unit>>
        {
            private readonly ICosmosDbService _cosmosDbService;

            public DeletePositionHandler(ICosmosDbService CosmosDbService)
                => _cosmosDbService = CosmosDbService;

            public async Task<Result<Unit>> Handle(CommandDelete request, CancellationToken cancellationToken)
            {
                var positionForDeleting = await _cosmosDbService.GetItem<Position>(request.Id, cancellationToken);
                if (positionForDeleting != null)
                {
                    int numberPosition = positionForDeleting.NumberInTheQueue;
                    var query = await _cosmosDbService.GetItems<Position>($"select * from position p WHERE p.number >= {numberPosition}", cancellationToken);
                    query.ForEach(pos => pos.NumberInTheQueue--);

                    foreach (var position in query)
                    {
                        await _cosmosDbService.UpdateItem(position.Id, position, cancellationToken);
                    }
                }
                else
                {
                    return Result<Unit>.Failure("Failed to find the position");
                }

                var result = await _cosmosDbService.DeleteItem<Position>(request.Id, cancellationToken);
                if (result == System.Net.HttpStatusCode.OK)
                {
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    return Result<Unit>.Failure("Failed to delete the position");
                }
            }
        }
    } 
}
