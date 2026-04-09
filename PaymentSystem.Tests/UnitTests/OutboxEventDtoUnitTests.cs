using FluentAssertions;
using PaymentSystem.Shared.Dtos.MappingDtos.OutboxEventDto;

namespace PaymentSystem.Tests.UnitTests
{
    public class OutboxEventDtoUnitTests
    {
        [Fact]
        public void OutboxEventGetDto_CanInitializeProperties()
        {
            var dto = new OutboxEventGetDto
            {
                Id = 1,
                EntityType = "Payment",
                EventType = "Created",
                Payload = "{\"id\": 1}",
                IsProcessed = true,
                ProcessedDate = new DateTime(2024, 1, 2),
                CreatedDate = new DateTime(2024, 1, 1),
                IsDeleted = false,
                IsActive = true
            };

            dto.Id.Should().Be(1);
            dto.EntityType.Should().Be("Payment");
            dto.EventType.Should().Be("Created");
            dto.Payload.Should().Be("{\"id\": 1}");
            dto.IsProcessed.Should().BeTrue();
            dto.ProcessedDate.Should().Be(new DateTime(2024, 1, 2));
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void OutboxEventGetDto_DefaultValues_AreCorrect()
        {
            var dto = new OutboxEventGetDto();
            dto.IsProcessed.Should().BeFalse();
            dto.IsDeleted.Should().BeFalse();
            dto.IsActive.Should().BeTrue();
        }
    }
}
