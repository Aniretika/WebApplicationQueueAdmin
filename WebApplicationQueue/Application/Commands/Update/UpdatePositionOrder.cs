using FluentValidation;
using MediatR;
using QueueWebApplicationTest.Application.Queue;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationQueue.Application.Core;
using WebApplicationQueue.Entities;
using WebApplicationQueue.Service;

namespace WebApplicationQueue.Application.Commands.Update
{
    public class UpdatePosition
    {
        public class CommandUpdate : IRequest<Result<Unit>>
        {
            public Position Position { get; set; }
        }

        public class CommandValidator : AbstractValidator<CommandUpdate>
        {
            public CommandValidator()
            {
                //TODO: validate from depency injection
                RuleFor(x => x.Position).SetValidator(new QueueValidator()!);
            }
        }
        public class UpdatePositionHandler : IRequestHandler<CommandUpdate, Result<Unit>>
        {
            private readonly ICosmosDbService _cosmosDbService;

            public UpdatePositionHandler(ICosmosDbService CosmosDbService)
                => _cosmosDbService = CosmosDbService;

            public async Task<Result<Unit>> Handle(CommandUpdate request, CancellationToken cancellationToken)
            {
                var oldPosition = await _cosmosDbService.GetItem<Position>(request.Position.Id, cancellationToken);
                int oldNumberPosition = oldPosition.NumberInTheQueue;
                int newNumberPosition = request.Position.NumberInTheQueue;

                List<Position> positionsOffset = null;

                if (oldNumberPosition < newNumberPosition)
                {
                    positionsOffset = await _cosmosDbService.GetItems<Position>($"select * from position p WHERE p.number >= {oldNumberPosition} AND p.number <= {newNumberPosition} AND p.id != \"{request.Position.Id}\"", cancellationToken);
                    positionsOffset.ForEach(pos => pos.NumberInTheQueue--);
                }
                else if (oldNumberPosition > newNumberPosition)
                {
                    positionsOffset = await _cosmosDbService.GetItems<Position>($"select * from position p WHERE p.number <= {oldNumberPosition} AND p.number >= {newNumberPosition} AND p.id != \"{request.Position.Id}\"", cancellationToken);
                    positionsOffset.ForEach(pos => pos.NumberInTheQueue++);
                }

                if (positionsOffset != null)
                {
                    foreach (var position in positionsOffset)
                    {
                        await _cosmosDbService.UpdateItem(position.Id, position, cancellationToken);
                    }
                }
                
                var result = await _cosmosDbService.UpsertItem(request.Position.Id, request.Position, cancellationToken);
                if (result == System.Net.HttpStatusCode.OK)
                {
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    return Result<Unit>.Failure("Failed to update position");
                }
            }
        }
    }
}
