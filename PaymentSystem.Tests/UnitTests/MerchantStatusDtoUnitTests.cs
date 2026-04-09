using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class MerchantStatusDtoUnitTests
    {
        [Fact]
        public void MerchantStatusCreateDto_CanInitializeProperties()
        {
            var dto = new MerchantStatusCreateDto
            {
                Name = "Active",
                Description = "Active merchant status",
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Name.Should().Be("Active");
            dto.Description.Should().Be("Active merchant status");
            dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void MerchantStatusUpdateDto_CanInitializeProperties()
        {
            var dto = new MerchantStatusUpdateDto
            {
                Id = 1,
                Name = "Inactive",
                Description = "Inactive merchant status",
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Inactive");
            dto.Description.Should().Be("Inactive merchant status");
            dto.UpdatedDate.Should().Be(new DateTime(2024, 6, 1));
        }

        [Fact]
        public void MerchantStatusGetDto_CanInitializeProperties()
        {
            var dto = new MerchantStatusGetDto
            {
                Id = 1,
                Name = "Active",
                Description = "Active status",
                MerchantCount = 15,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Active");
            dto.MerchantCount.Should().Be(15);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void MerchantStatusGetDto_DefaultValues_AreCorrect()
        {
            var dto = new MerchantStatusGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void MerchantStatusCreateDto_Description_CanBeNull()
        {
            var dto = new MerchantStatusCreateDto
            {
                Name = "Test",
                Description = null
            };
            dto.Description.Should().BeNull();
        }
    }
}
