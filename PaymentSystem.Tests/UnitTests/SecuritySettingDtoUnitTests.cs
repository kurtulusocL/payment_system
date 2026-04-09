using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class SecuritySettingDtoUnitTests
    {
        [Fact]
        public void SecuritySettingCreateDto_CanInitializeProperties()
        {
            var dto = new SecuritySettingCreateDto
            {
                Type = "MaxLoginAttempts",
                Value = "5",
                CreatedDate = new DateTime(2024, 1, 1)
            };

            dto.Type.Should().Be("MaxLoginAttempts");
            dto.Value.Should().Be("5");
            dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void SecuritySettingUpdateDto_CanInitializeProperties()
        {
            var dto = new SecuritySettingUpdateDto
            {
                Id = 1,
                Type = "SessionTimeout",
                Value = "30",
                UpdatedDate = new DateTime(2024, 6, 1)
            };

            dto.Id.Should().Be(1);
            dto.Type.Should().Be("SessionTimeout");
            dto.Value.Should().Be("30");
            dto.UpdatedDate.Should().Be(new DateTime(2024, 6, 1));
        }

        [Fact]
        public void SecuritySettingGetDto_CanInitializeProperties()
        {
            var dto = new SecuritySettingGetDto
            {
                Id = 1,
                Type = "PasswordExpiry",
                Value = "90",
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Type.Should().Be("PasswordExpiry");
            dto.Value.Should().Be("90");
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void SecuritySettingGetDto_DefaultValues_AreCorrect()
        {
            var dto = new SecuritySettingGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }
    }
}
