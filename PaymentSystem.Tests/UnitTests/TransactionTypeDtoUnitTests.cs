using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;

namespace PaymentSystem.Tests.UnitTests
{
    public class TransactionTypeDtoUnitTests
    {
        [Fact]
        public void TransactionTypeCreateDto_CanInitializeProperties()
        {
            var dto = new TransactionTypeCreateDto
            {
                Name = "Deposit",
                Description = "Deposit transaction type",
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Name.Should().Be("Deposit");
            dto.Description.Should().Be("Deposit transaction type");
            dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void TransactionTypeUpdateDto_CanInitializeProperties()
        {
            var dto = new TransactionTypeUpdateDto
            {
                Id = 1,
                Name = "Withdrawal",
                Description = "Withdrawal transaction type",
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Withdrawal");
            dto.Description.Should().Be("Withdrawal transaction type");
            dto.UpdatedDate.Should().Be(new DateTime(2024, 6, 1));
        }

        [Fact]
        public void TransactionTypeGetDto_CanInitializeProperties()
        {
            var dto = new TransactionTypeGetDto
            {
                Id = 1,
                Name = "Transfer",
                Description = "Transfer transaction type",
                TransactionCount = 50,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Transfer");
            dto.TransactionCount.Should().Be(50);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void TransactionTypeGetDto_DefaultValues_AreCorrect()
        {
            var dto = new TransactionTypeGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void TransactionTypeCreateDto_Description_CanBeNull()
        {
            var dto = new TransactionTypeCreateDto
            {
                Name = "Test",
                Description = null
            };
            dto.Description.Should().BeNull();
        }
    }
}
