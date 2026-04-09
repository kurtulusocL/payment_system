using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class UserSessionDtoUnitTests
    {
        [Fact]
        public void UserSessionGetDto_CanInitializeProperties()
        {
            var dto = new UserSessionGetDto
            {
                Id = 1,
                Username = "testuser",
                LoginDate = new DateTime(2024, 1, 1, 10, 0, 0),
                LogoutDate = new DateTime(2024, 1, 1, 11, 0, 0),
                IsOnline = false,
                OnlineDurationSeconds = 3600,
                AppUserId = "user-123",
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.Username.Should().Be("testuser");
            dto.LoginDate.Should().Be(new DateTime(2024, 1, 1, 10, 0, 0));
            dto.LogoutDate.Should().Be(new DateTime(2024, 1, 1, 11, 0, 0));
            dto.IsOnline.Should().BeFalse();
            dto.OnlineDurationSeconds.Should().Be(3600);
            dto.AppUserId.Should().Be("user-123");
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void UserSessionGetDto_DefaultValues_AreCorrect()
        {
            var dto = new UserSessionGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void UserSessionGetDto_OnlineSession_CanInitialize()
        {
            var dto = new UserSessionGetDto
            {
                Username = "onlineuser",
                LoginDate = DateTime.UtcNow,
                LogoutDate = null,
                IsOnline = true,
                AppUserId = "user-456"
            };

            dto.IsOnline.Should().BeTrue();
            dto.LogoutDate.Should().BeNull();
        }
    }
}
