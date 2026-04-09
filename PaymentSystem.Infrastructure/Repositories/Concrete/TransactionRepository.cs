using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;
using PaymentSystem.Infrastructure.GenericRepository.EntityFramework;
using PaymentSystem.Infrastructure.Repositories.Abstract;

namespace PaymentSystem.Infrastructure.Repositories.Concrete
{
    public class TransactionRepository : EntityRepositoryBase<Transaction, ApplicationDbContext>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
