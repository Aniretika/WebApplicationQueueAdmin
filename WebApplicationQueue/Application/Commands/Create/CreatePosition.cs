using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationQueue.Application.Core;
using WebApplicationQueue.Entities;
using WebApplicationQueue.Service;

namespace QueueWebApplicationTest.Application.Queue
{
    public class CreatePosition
    {
        public class CommandCreate : IRequest<Result<Unit>>
        {
            public Position Position { get; set; }
        }

        public class CommandValidator : AbstractValidator<CommandCreate>
        {
            public CommandValidator()
            {
                RuleFor(queue => queue.Position).SetValidator(new QueueValidator()!);
            }
        }

        public class CreatePositionHandler : IRequestHandler<CommandCreate, Result<Unit>>
        {

            private readonly ICosmosDbService _cosmosDbService;

            public CreatePositionHandler(ICosmosDbService cosmosDbService)
            {
                _cosmosDbService = cosmosDbService;
            }
              

            public async Task<Result<Unit>> Handle(CommandCreate request, CancellationToken cancellationToken)
            {
                var LNewGuid = Guid.NewGuid();
                int lastNumberInQueue;
                var positions = await _cosmosDbService.GetItems<Position>($"select * from {nameof(Position)}", cancellationToken);
                
                if (positions.Count == 0) { lastNumberInQueue = -1; }
                else
                {
                    lastNumberInQueue = positions.Max(pos => pos.NumberInTheQueue);
                }
                var result = await _cosmosDbService.AddItem(LNewGuid, new Position
                {
                    Id = LNewGuid,
                    AuthorId = request.Position.AuthorId,
                    BotId = request.Position.BotId,
                    NumberInTheQueue = lastNumberInQueue + 1,
                    Requester = request.Position.Requester,
                    Description = request.Position.Description,
                    RegistrationTime = DateTime.Now
                }, cancellationToken);

                if (result == System.Net.HttpStatusCode.OK)
                {
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    return Result<Unit>.Failure("Failed to create position");
                }

            }
        }
    }
}
