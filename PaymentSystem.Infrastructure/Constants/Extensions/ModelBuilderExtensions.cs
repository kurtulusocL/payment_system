using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Application.Constants.Utilities;
using PaymentSystem.Domain.Entities;

namespace PaymentSystem.Infrastructure.Constants.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureDbContexts(this ModelBuilder builder, EncryptionConverter converter, bool includeLocalEntities = false)
        {
            builder.Entity<AppUser>(e =>
            {
                e.ToTable("AppUsers");
                e.HasIndex(x => x.Email).IsUnique();
                e.HasIndex(x => x.UserName).IsUnique();
                e.Property(x => x.NameSurname).IsRequired().HasMaxLength(150);
                e.Property(x => x.Title).HasMaxLength(100);
                e.Property(x => x.ConfirmCode).HasMaxLength(10);
                e.Property(x => x.PhoneNumber).HasConversion(converter);
                e.Property(x => x.Email).HasConversion(converter);
                e.Property(x => x.NameSurname).HasConversion(converter);
                e.Property(x => x.UserName).HasConversion(converter);

                e.HasIndex(x => x.Id).HasDatabaseName("IX_AppUser_Id").IsUnique();
                e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_AppUser_IsActive_IsDeleted");
            });

            builder.Entity<AppRole>(e =>
            {
                e.ToTable("AppRoles");
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Id).HasDatabaseName("IX_AppRole_Id").IsUnique();
                e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_AppRole_IsActive_IsDeleted");
            });

            builder.Entity<IdentityUserRole<string>>().ToTable("AppUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AppUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AppUserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AppRoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("AppUserTokens");

            builder.Entity<Currency>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Code).IsRequired().HasMaxLength(10);
                e.Property(x => x.Symbol).HasMaxLength(5);
            });

            builder.Entity<PaymentStatus>(e =>
            {
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            });

            builder.Entity<TransactionType>(e =>
            {
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            });

            builder.Entity<MerchantStatus>(e =>
            {
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            });

            builder.Entity<Merchant>(e =>
            {
                e.HasIndex(x => x.Email).IsUnique();
                e.HasIndex(x => x.ApiKey).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.Property(x => x.PhoneNumber).HasMaxLength(500);
                e.Property(x => x.TaxNumber).HasMaxLength(500);
                e.HasOne(x => x.MerchantStatus).WithMany(x => x.Merchants).HasForeignKey(x => x.MerchantStatusId).OnDelete(DeleteBehavior.Restrict);
                e.Property(x => x.PhoneNumber).HasConversion(converter);
                e.Property(x => x.TaxNumber).HasConversion(converter);
            });

            builder.Entity<Payment>(e =>
            {
                e.HasIndex(x => x.IdempotencyKey).IsUnique();
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.MaskedCardNumber).HasMaxLength(500);
                e.Property(x => x.IdempotencyKey).IsRequired().HasMaxLength(100);
                e.HasOne(x => x.User).WithMany(x => x.Payments).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Merchant).WithMany(x => x.Payments).HasForeignKey(x => x.MerchantId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Currency).WithMany(x => x.Payments).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.PaymentStatus).WithMany(x => x.Payments).HasForeignKey(x => x.PaymentStatusId).OnDelete(DeleteBehavior.Restrict);
                e.Property(x => x.MaskedCardNumber).HasConversion(converter);
            });

            builder.Entity<Wallet>(e =>
            {
                e.HasIndex(x => x.UserId).IsUnique();
                e.Property(x => x.Balance).HasColumnType("decimal(18,2)");
                e.Property(x => x.RowVersion).IsRowVersion();
                e.HasOne(x => x.User).WithOne(x => x.Wallet).HasForeignKey<Wallet>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Currency).WithMany(x => x.Wallets).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Transaction>(e =>
            {
                e.HasIndex(x => x.Id).HasDatabaseName("IX_Transaction_Id").IsUnique();
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Reference).HasMaxLength(200);
                e.HasOne(x => x.Wallet).WithMany(x => x.Transactions).HasForeignKey(x => x.WalletId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Payment).WithMany(x => x.Transactions).HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Currency).WithMany(x => x.Transactions).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TransactionType).WithMany(x => x.Transactions).HasForeignKey(x => x.TransactionTypeId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UserSession>(e =>
            {
                e.Property(x => x.LoginDate).IsRequired();
                e.HasOne(x => x.User).WithMany(x => x.UserSessions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => x.Id).HasDatabaseName("IX_UserSession_Id").IsUnique();
                e.HasIndex(x => x.UserId).HasDatabaseName("IX_UserSession_UserId");
                e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_UserSession_IsActive_IsDeleted");
            });

            builder.Entity<Audit>(e =>
            {
                e.HasIndex(x => x.Id).HasDatabaseName("IX_Audit_Id").IsUnique();
                e.HasIndex(x => x.AppUserId).HasDatabaseName("IX_Audit_AppUserId");
                e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_Audit_IsActive_IsDeleted");
                e.HasOne(x => x.AppUser).WithMany(x => x.Audits).HasForeignKey(x => x.AppUserId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ExceptionLogger>(e =>
            {
                e.HasIndex(x => x.Id).HasDatabaseName("IX_ExceptionLogger_Id").IsUnique();
                e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_ExceptionLogger_IsActive_IsDeleted");
            });

            builder.Entity<SecuritySetting>(e =>
            {
                e.HasIndex(x => x.Id).IsUnique();
                e.HasIndex(x => x.Type).HasDatabaseName("IX_SecuritySetting_Type");
                e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_SecuritySetting_IsActive_IsDeleted");
                e.Property(x => x.Type).IsRequired().HasMaxLength(100);
                e.Property(x => x.Value).IsRequired().HasMaxLength(500);
            });

            if (includeLocalEntities)
            {
                builder.Entity<OutboxEvent>(e =>
                {
                    e.HasIndex(x => x.IsProcessed).HasDatabaseName("IX_OutboxEvent_IsProcessed");
                    e.HasIndex(x => x.Id).HasDatabaseName("IX_OutboxEvent_Id").IsUnique();
                    e.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_OutboxEvent_IsActive_IsDeleted");
                    e.Property(x => x.EntityType).IsRequired().HasMaxLength(200);
                    e.Property(x => x.EventType).IsRequired().HasMaxLength(50);
                    e.Property(x => x.Payload).IsRequired();
                });
            }
        }
    }
}
