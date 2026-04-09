using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class MerchantDtoUnitTests
    {
        [Fact]
        public void MerchantCreateDto_CanInitializeProperties()
        {
            var apiKey = Guid.NewGuid();
            var dto = new MerchantCreateDto
            {
                Name = "Test Merchant",
                Email = "merchant@test.com",
                PhoneNumber = "+1234567890",
                TaxNumber = "1234567890",
                ApiKey = apiKey,
                MerchantStatusId = 1,
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Name.Should().Be("Test Merchant");
            dto.Email.Should().Be("merchant@test.com");
            dto.PhoneNumber.Should().Be("+1234567890");
            dto.TaxNumber.Should().Be("1234567890");
            dto.ApiKey.Should().Be(apiKey);
            dto.MerchantStatusId.Should().Be(1);
        }

        [Fact]
        public void MerchantUpdateDto_CanInitializeProperties()
        {
            var apiKey = Guid.NewGuid();
            var dto = new MerchantUpdateDto
            {
                Id = 1,
                Name = "Updated Merchant",
                Email = "updated@test.com",
                PhoneNumber = "+0987654321",
                TaxNumber = "0987654321",
                ApiKey = apiKey,
                MerchantStatusId = 2,
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Updated Merchant");
            dto.Email.Should().Be("updated@test.com");
            dto.PhoneNumber.Should().Be("+0987654321");
            dto.TaxNumber.Should().Be("0987654321");
            dto.ApiKey.Should().Be(apiKey);
            dto.MerchantStatusId.Should().Be(2);
        }

        [Fact]
        public void MerchantGetDto_CanInitializeProperties()
        {
            var apiKey = Guid.NewGuid();
            var dto = new MerchantGetDto
            {
                Id = 1,
                Name = "Merchant Get",
                Email = "get@test.com",
                PhoneNumber = "+1111111111",
                TaxNumber = "1111111111",
                ApiKey = apiKey,
                MerchantStatusId = 1,
                PaymentCount = 30,
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Merchant Get");
            dto.Email.Should().Be("get@test.com");
            dto.PaymentCount.Should().Be(30);
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void MerchantGetDto_DefaultValues_AreCorrect()
        {
            var dto = new MerchantGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void MerchantCreateDto_OptionalFields_CanBeNull()
        {
            var dto = new MerchantCreateDto
            {
                Name = "Test",
                Email = "test@test.com",
                PhoneNumber = null,
                TaxNumber = null
            };

            dto.PhoneNumber.Should().BeNull();
            dto.TaxNumber.Should().BeNull();
        }
    }
}
