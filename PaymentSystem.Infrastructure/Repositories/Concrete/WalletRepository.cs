using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;
using PaymentSystem.Infrastructure.GenericRepository.EntityFramework;
using PaymentSystem.Infrastructure.Repositories.Abstract;

namespace PaymentSystem.Infrastructure.Repositories.Concrete
{
    public class WalletRepository : EntityRepositoryBase<Wallet, ApplicationDbContext>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
