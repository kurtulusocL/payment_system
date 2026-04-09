using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class PaymentStatusDtoUnitTests
    {
        [Fact]
        public void PaymentStatusCreateDto_CanInitializeProperties()
        {
            var dto = new PaymentStatusCreateDto
            {
                Name = "Completed",
                Description = "Payment completed successfully",
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Name.Should().Be("Completed");
            dto.Description.Should().Be("Payment completed successfully");
            dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void PaymentStatusUpdateDto_CanInitializeProperties()
        {
            var dto = new PaymentStatusUpdateDto
            {
                Id = 1,
                Name = "Failed",
                Description = "Payment failed",
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Failed");
            dto.Description.Should().Be("Payment failed");
            dto.UpdatedDate.Should().Be(new DateTime(2024, 6, 1));
        }

        [Fact]
        public void PaymentStatusGetDto_CanInitializeProperties()
        {
            var dto = new PaymentStatusGetDto
            {
                Id = 1,
                Name = "Pending",
                Description = "Payment pending",
                PaymentCount = 25,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Pending");
            dto.PaymentCount.Should().Be(25);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void PaymentStatusGetDto_DefaultValues_AreCorrect()
        {
            var dto = new PaymentStatusGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void PaymentStatusCreateDto_Description_CanBeNull()
        {
            var dto = new PaymentStatusCreateDto
            {
                Name = "Test",
                Description = null
            };
            dto.Description.Should().BeNull();
        }
    }
}
