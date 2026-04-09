using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.GenericRepository;

namespace PaymentSystem.Infrastructure.Repositories.Abstract
{
    public interface IOutboxEventRepository : IEntityRepository<OutboxEvent>
    {
    }
}
