using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;

namespace PaymentSystem.Tests.UnitTests
{
    public class ExceptionLoggerDtoUnitTests
    {
        [Fact]
        public void ExceptionLoggerGetDto_CanInitializeProperties()
        {
            var dto = new ExceptionLoggerGetDto
            {
                Id = 1,
                ExceptionMessage = "Null reference exception",
                ControllerName = "PaymentsController",
                ExceptionStackTrace = "at PaymentSystem...",
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true,
                UpdatedDate = null,
                DeletedDate = null,
                SuspendedDate = null
            };

            dto.Id.Should().Be(1);
            dto.ExceptionMessage.Should().Be("Null reference exception");
            dto.ControllerName.Should().Be("PaymentsController");
            dto.ExceptionStackTrace.Should().Be("at PaymentSystem...");
            dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void ExceptionLoggerGetDto_DefaultValues_AreCorrect()
        {
            var dto = new ExceptionLoggerGetDto();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }
    }
}
