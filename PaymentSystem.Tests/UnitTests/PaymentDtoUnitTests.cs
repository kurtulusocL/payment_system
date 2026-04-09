using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class PaymentDtoUnitTests
    {
        [Fact]
        public void PaymentCreateDto_CanInitializeProperties()
        {
            var dto = new PaymentCreateDto
            {
                Amount = 100.50m,
                IdempotencyKey = "idem-key-123",
                Description = "Test payment",
                MaskedCardNumber = "****1234",
                UserId = "user-1",
                MerchantId = 1,
                CurrencyId = 1,
                PaymentStatusId = 1,
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Amount.Should().Be(100.50m);
            dto.IdempotencyKey.Should().Be("idem-key-123");
            dto.Description.Should().Be("Test payment");
            dto.MaskedCardNumber.Should().Be("****1234");
            dto.UserId.Should().Be("user-1");
            dto.MerchantId.Should().Be(1);
            dto.CurrencyId.Should().Be(1);
            dto.PaymentStatusId.Should().Be(1);
        }

        [Fact]
        public void PaymentUpdateDto_CanInitializeProperties()
        {
            var dto = new PaymentUpdateDto
            {
                Id = 1,
                Amount = 200.75m,
                IdempotencyKey = "idem-key-456",
                Description = "Updated payment",
                MaskedCardNumber = "****5678",
                UserId = "user-2",
                MerchantId = 2,
                CurrencyId = 2,
                PaymentStatusId = 2,
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Amount.Should().Be(200.75m);
            dto.IdempotencyKey.Should().Be("idem-key-456");
            dto.UserId.Should().Be("user-2");
            dto.MerchantId.Should().Be(2);
        }

        [Fact]
        public void PaymentGetDto_CanInitializeProperties()
        {
            var dto = new PaymentGetDto
            {
                Id = 1,
                Amount = 150.00m,
                IdempotencyKey = "idem-key-789",
                Description = "Get payment",
                MaskedCardNumber = "****9012",
                UserId = "user-3",
                MerchantId = 3,
                CurrencyId = 3,
                PaymentStatusId = 3,
                TransactionCount = 5,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Amount.Should().Be(150.00m);
            dto.TransactionCount.Should().Be(5);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void PaymentGetDto_DefaultValues_AreCorrect()
        {
            var dto = new PaymentGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void PaymentCreateDto_OptionalFields_CanBeNull()
        {
            var dto = new PaymentCreateDto
            {
                Amount = 50m,
                IdempotencyKey = "key",
                UserId = "u1",
                MerchantId = 1,
                CurrencyId = 1,
                PaymentStatusId = 1,
                Description = null,
                MaskedCardNumber = null
            };

            dto.Description.Should().BeNull();
            dto.MaskedCardNumber.Should().BeNull();
        }
    }
}
