using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class WalletDtoUnitTests
    {
        [Fact]
        public void WalletCreateDto_CanInitializeProperties()
        {
            var dto = new WalletCreateDto
            {
                Balance = 1000.50m,
                RowVersion = new byte[] { 0, 0, 0, 1 },
                UserId = "user-1",
                CurrencyId = 1,
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Balance.Should().Be(1000.50m);
            dto.RowVersion.Should().Equal(new byte[] { 0, 0, 0, 1 });
            dto.UserId.Should().Be("user-1");
            dto.CurrencyId.Should().Be(1);
        }

        [Fact]
        public void WalletUpdateDto_CanInitializeProperties()
        {
            var dto = new WalletUpdateDto
            {
                Id = 1,
                Balance = 2000.75m,
                RowVersion = new byte[] { 0, 0, 0, 2 },
                UserId = "user-2",
                CurrencyId = 2,
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Balance.Should().Be(2000.75m);
            dto.UserId.Should().Be("user-2");
            dto.CurrencyId.Should().Be(2);
        }

        [Fact]
        public void WalletGetDto_CanInitializeProperties()
        {
            var dto = new WalletGetDto
            {
                Id = 1,
                Balance = 500.00m,
                RowVersion = new byte[] { 0, 0, 0, 3 },
                UserId = "user-3",
                CurrencyId = 3,
                TransactionCount = 10,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Balance.Should().Be(500.00m);
            dto.TransactionCount.Should().Be(10);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void WalletGetDto_DefaultValues_AreCorrect()
        {
            var dto = new WalletGetDto();
            dto.Balance.Should().Be(0);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void WalletCreateDto_RowVersion_CanBeNull()
        {
            var dto = new WalletCreateDto
            {
                Balance = 100m,
                UserId = "u1",
                CurrencyId = 1,
                RowVersion = null
            };

            dto.RowVersion.Should().BeNull();
        }
    }
}
