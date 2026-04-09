using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.OutboxEventDto;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class OutboxEventsControllerMoqTests
    {
        private readonly Mock<IOutboxEventService> _m;
        private readonly OutboxEventsController _c;

        public OutboxEventsControllerMoqTests()
        {
            _m = new Mock<IOutboxEventService>();
            _c = new OutboxEventsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAll()).Returns(new List<OutboxEventGetDto>().AsQueryable());
            _c.GetAllOutboxEvents().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetSuccessful_ReturnsOk()
        {
            _m.Setup(x => x.GetAllBySuccessfullProcess()).Returns(new List<OutboxEventGetDto>().AsQueryable());
            _c.GetAllSuccessfulOutboxEvents().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetFailed_ReturnsOk()
        {
            _m.Setup(x => x.GetAllByErrorProcess()).Returns(new List<OutboxEventGetDto>().AsQueryable());
            _c.GetAllFailedOutboxEvents().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllForAdmin()).Returns(new List<OutboxEventGetDto>().AsQueryable());
            _c.GetAllOutboxEventsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new OutboxEventGetDto());
            (await _c.GetOutboxEventById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((OutboxEventGetDto?)null);
            (await _c.GetOutboxEventById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteOutboxEvent(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.DeleteOutboxEvent(1)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteOutboxEventsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetActiveAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SetActive(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInactive_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetInActiveAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SetInactive(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SoftDelete_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetDeletedAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SoftDelete(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Restore_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetNotDeletedAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.Restore(1)).Should().BeOfType<OkObjectResult>();
        }
    }
}
