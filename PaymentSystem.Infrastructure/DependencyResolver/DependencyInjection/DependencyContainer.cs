using System.Reflection;
using FluentValidation;
using Ganss.Xss;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Application.Constants.Caching.MemoryCache;
using PaymentSystem.Application.Constants.Caching.Redis;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Constants.Services.Concrete;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Application.Validators.Extention;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Infrastructure.Constants.Handlers;
using PaymentSystem.Infrastructure.Constants.Interceptors;
using PaymentSystem.Infrastructure.Constants.Workers;
using PaymentSystem.Infrastructure.Data.Context.Azure;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;
using PaymentSystem.Infrastructure.GenericRepository;
using PaymentSystem.Infrastructure.GenericRepository.Azure;
using PaymentSystem.Infrastructure.Repositories.Abstract;
using PaymentSystem.Infrastructure.Repositories.Concrete;
using PaymentSystem.Infrastructure.Services.Concrete;
using PaymentSystem.Shared.Factory;
using PaymentSystem.Shared.Settings;
using StackExchange.Redis;

namespace PaymentSystem.Infrastructure.DependencyResolver.DependencyInjection
{
    public static class DependencyContainer
    {
        public static void DependencyService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            var redisConnection = configuration.GetConnectionString("Redis");

            if (!string.IsNullOrEmpty(redisConnection))
            {
                try
                {
                    var multiplexer = ConnectionMultiplexer.Connect(redisConnection);
                    services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                    services.AddSingleton<ICacheService, RedisManager>();
                }
                catch (Exception)
                {
                    services.AddMemoryCache();
                    services.AddSingleton<ICacheService, MemoryCacheManager>();
                }
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheManager>();
            }

            string localConnection = configuration.GetConnectionString("DefaultConnection")!;
            string azureConnection = configuration.GetConnectionString("AzureConnection")!;

            services.AddSingleton<OutboxEventInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(localConnection, sqlOptions =>
                {
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                })
                .AddInterceptors(sp.GetRequiredService<OutboxEventInterceptor>());
            });
            services.AddDbContext<AzureDbContext>(options =>
            {
                options.UseSqlServer(azureConnection, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });

            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequiredLength = 8;
                opt.Lockout.AllowedForNewUsers = true;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 4;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddHostedService<AzureSyncWorker>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAzureService, AzureBaseService>();
            services.AddScoped<JwtAuthorizationHandler>();

            
            services.AddScoped<IAuthService, AuthManager>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();

            services.AddScoped<IAppRoleRepository, AppRoleRepository>();

            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IAuditService, AuditManager>();

            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ICurrencyService, CurrencyManager>();

            services.AddScoped<IExceptionLoggerRepository, ExceptionLoggerRepository>();
            services.AddScoped<IExceptionLoggerService, ExceptionLoggerManager>();

            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IMerchantService, MerchantManager>();

            services.AddScoped<IMerchantStatusRepository, MerchantStatusRepository>();
            services.AddScoped<IMerchantStatusService, MerchantStatusManager>();

            services.AddScoped<IOutboxEventRepository, OutboxEventRepository>();
            services.AddScoped<IOutboxEventService, OutboxEventManager>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentService, PaymentManager>();

            services.AddScoped<IPaymentStatusRepository, PaymentStatusRepository>();
            services.AddScoped<IPaymentStatusService, PaymentStatusManager>();

            services.AddScoped<ISecuritySettingRepository, SecuritySettingRepository>();
            services.AddScoped<ISecuritySettingService, SecuritySettingManager>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionService, TransactionManager>();

            services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
            services.AddScoped<ITransactionTypeService, TransactionTypeManager>();

            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IUserSessionService, UserSessionManager>();

            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletService, WalletManager>();

            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IUserService, UserManager>();

            services.AddScoped<IAppRoleRepository, AppRoleRepository>();
            services.AddScoped<IRoleService, RoleManager>();

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddSingleton<IHtmlSanitizer>(_ => HtmlSanitizerFactory.Create());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationServices();
        }
    }
}
