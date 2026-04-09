using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class CurrencyDtoUnitTests
    {
        [Fact]
        public void CurrencyCreateDto_CanInitializeProperties()
        {
            var dto = new CurrencyCreateDto
            {
                Name = "US Dollar",
                Code = "USD",
                Symbol = "$",
                Description = "United States Dollar",
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Name.Should().Be("US Dollar");
            dto.Code.Should().Be("USD");
            dto.Symbol.Should().Be("$");
            dto.Description.Should().Be("United States Dollar");
            dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void CurrencyUpdateDto_CanInitializeProperties()
        {
            var dto = new CurrencyUpdateDto
            {
                Id = 1,
                Name = "Euro",
                Code = "EUR",
                Symbol = "E",
                Description = "European Euro",
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Euro");
            dto.Code.Should().Be("EUR");
            dto.Symbol.Should().Be("E");
            dto.Description.Should().Be("European Euro");
            dto.UpdatedDate.Should().Be(new DateTime(2024, 6, 1));
        }

        [Fact]
        public void CurrencyGetDto_CanInitializeProperties()
        {
            var dto = new CurrencyGetDto
            {
                Id = 1,
                Name = "Turkish Lira",
                Code = "TRY",
                Symbol = "TL",
                Description = "Turkish Lira",
                PaymentCount = 10,
                WalletCount = 5,
                TransactionCount = 20,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true,
                UpdatedDate = null,
                DeletedDate = null,
                SuspendedDate = null
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Turkish Lira");
            dto.Code.Should().Be("TRY");
            dto.Symbol.Should().Be("TL");
            dto.Description.Should().Be("Turkish Lira");
            dto.PaymentCount.Should().Be(10);
            dto.WalletCount.Should().Be(5);
            dto.TransactionCount.Should().Be(20);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void CurrencyGetDto_DefaultValues_AreCorrect()
        {
            var dto = new CurrencyGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void CurrencyCreateDto_SymbolAndDescription_CanBeNull()
        {
            var dto = new CurrencyCreateDto
            {
                Name = "Test",
                Code = "TST",
                Symbol = null,
                Description = null
            };

            dto.Symbol.Should().BeNull();
            dto.Description.Should().BeNull();
        }
    }
}
