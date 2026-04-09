using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class TransactionDtoUnitTests
    {
        [Fact]
        public void TransactionCreateDto_CanInitializeProperties()
        {
            var dto = new TransactionCreateDto
            {
                Amount = 250.00m,
                Reference = "REF-123",
                WalletId = 1,
                PaymentId = 2,
                CurrencyId = 1,
                TransactionTypeId = 1,
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Amount.Should().Be(250.00m);
            dto.Reference.Should().Be("REF-123");
            dto.WalletId.Should().Be(1);
            dto.PaymentId.Should().Be(2);
            dto.CurrencyId.Should().Be(1);
            dto.TransactionTypeId.Should().Be(1);
        }

        [Fact]
        public void TransactionUpdateDto_CanInitializeProperties()
        {
            var dto = new TransactionUpdateDto
            {
                Id = 1,
                Amount = 300.50m,
                Reference = "REF-456",
                WalletId = 2,
                PaymentId = 3,
                CurrencyId = 2,
                TransactionTypeId = 2,
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Amount.Should().Be(300.50m);
            dto.Reference.Should().Be("REF-456");
            dto.WalletId.Should().Be(2);
        }

        [Fact]
        public void TransactionGetDto_CanInitializeProperties()
        {
            var dto = new TransactionGetDto
            {
                Id = 1,
                Amount = 175.25m,
                Reference = "REF-789",
                WalletId = 3,
                PaymentId = 4,
                CurrencyId = 3,
                TransactionTypeId = 3,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Amount.Should().Be(175.25m);
            dto.WalletId.Should().Be(3);
            dto.PaymentId.Should().Be(4);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void TransactionGetDto_DefaultValues_AreCorrect()
        {
            var dto = new TransactionGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void TransactionCreateDto_Reference_CanBeNull()
        {
            var dto = new TransactionCreateDto
            {
                Amount = 100m,
                WalletId = 1,
                CurrencyId = 1,
                TransactionTypeId = 1,
                Reference = null
            };

            dto.Reference.Should().BeNull();
        }

        [Fact]
        public void TransactionCreateDto_PaymentId_CanBeNull()
        {
            var dto = new TransactionCreateDto
            {
                Amount = 100m,
                WalletId = 1,
                CurrencyId = 1,
                TransactionTypeId = 1,
                PaymentId = null
            };

            dto.PaymentId.Should().BeNull();
        }
    }
}
