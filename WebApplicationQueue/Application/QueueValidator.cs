using FluentValidation;
using WebApplicationQueue.Entities;

namespace QueueWebApplicationTest.Application.Queue
{
    public class QueueValidator : AbstractValidator<Position>
    {
        public QueueValidator()
        {
            RuleFor(position => position.Requester).NotEmpty();
            RuleFor(position => position.Description).NotEmpty();
            RuleFor(position => position.NumberInTheQueue).NotEmpty();
            RuleFor(position => position.RegistrationTime).NotEmpty();
        }
    }
}
