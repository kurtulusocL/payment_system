using MediatR;

namespace PaymentSystem.Domain.Events.Abstract
{
    public interface IDomainEvent:INotification
    {
        DateTime OccurredOn { get; }
    }
}
