using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Application.Constants.Services.Concrete;
using PaymentSystem.Infrastructure.Constants.Extensions;
using System.IO;

namespace PaymentSystem.Infrastructure.Data.Context.Local.Mssql
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            var encryptionService = new EncryptionService(configuration);
            return new ApplicationDbContext(optionsBuilder.Options, encryptionService);
        }

        private static IConfiguration BuildConfiguration()
        {
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "PaymentSystem.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile(Path.Combine(apiProjectPath, "appsettings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(apiProjectPath, "appsettings.Development.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();

            return configuration;
        }
    }
}
