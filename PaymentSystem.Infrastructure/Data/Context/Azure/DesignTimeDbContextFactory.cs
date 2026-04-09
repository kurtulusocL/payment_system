using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PaymentSystem.Application.Constants.Services.Concrete;
using System.IO;

namespace PaymentSystem.Infrastructure.Data.Context.Azure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AzureDbContext>
    {
        public AzureDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var connectionString = configuration.GetConnectionString("AzureConnection")
                ?? throw new InvalidOperationException("Could not find a connection string named 'AzureConnection'.");

            var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var encryptionService = new EncryptionService(configuration);
            return new AzureDbContext(optionsBuilder.Options, encryptionService);
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
