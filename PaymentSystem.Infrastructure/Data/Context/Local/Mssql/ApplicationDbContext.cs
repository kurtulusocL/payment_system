using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Constants.Utilities;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Constants.Extensions;

namespace PaymentSystem.Infrastructure.Data.Context.Local.Mssql
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly IEncryptionService _encryptionService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEncryptionService encryptionService) : base(options) 
        {
            _encryptionService = encryptionService;
        }

        public DbSet<ExceptionLogger> ExceptionLoggers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<MerchantStatus> MerchantStatuses { get; set; }
        public DbSet<SecuritySetting> SecuritySettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var converter = new EncryptionConverter(_encryptionService);
            modelBuilder.ConfigureDbContexts(converter, includeLocalEntities: true);
        }
    }
}
