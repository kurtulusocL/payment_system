using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;
using PaymentSystem.Infrastructure.GenericRepository.EntityFramework;
using PaymentSystem.Infrastructure.Repositories.Abstract;

namespace PaymentSystem.Infrastructure.Repositories.Concrete
{
    public class ExceptionLoggerRepository : EntityRepositoryBase<ExceptionLogger, ApplicationDbContext>, IExceptionLoggerRepository
    {
        public ExceptionLoggerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
