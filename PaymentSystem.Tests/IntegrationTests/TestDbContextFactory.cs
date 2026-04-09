using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Infrastructure.Data.Context.Local.Mssql;

namespace PaymentSystem.Tests.IntegrationTests
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var mockEncryption = new Mock<IEncryptionService>();
            mockEncryption.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string s) => s);
            mockEncryption.Setup(x => x.Decrypt(It.IsAny<string>())).Returns((string s) => s);

            return new ApplicationDbContext(options, mockEncryption.Object);
        }
    }
}
