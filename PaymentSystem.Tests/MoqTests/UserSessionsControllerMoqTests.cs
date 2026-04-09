using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class UserSessionsControllerMoqTests
    {
        private readonly Mock<IUserSessionService> _m;
        private readonly UserSessionsController _c;

        public UserSessionsControllerMoqTests()
        {
            _m = new Mock<IUserSessionService>();
            _c = new UserSessionsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<UserSessionGetDto>().AsQueryable());
            _c.GetAllUserSessions().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetOnline_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByOnline()).Returns(new List<UserSessionGetDto>().AsQueryable());
            _c.GetAllOnlineUserSessions().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByUser_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByUserId("user1")).Returns(new List<UserSessionGetDto>().AsQueryable());
            _c.GetAllUserSessionsByUserId("user1").Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByLoginDate_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByLoginDate()).Returns(new List<UserSessionGetDto>().AsQueryable());
            _c.GetAllUserSessionsByLoginDate().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByDuration_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByOnlineDurationTime()).Returns(new List<UserSessionGetDto>().AsQueryable());
            _c.GetAllUserSessionsByOnlineDuration().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<UserSessionGetDto>().AsQueryable());
            _c.GetAllUserSessionsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new UserSessionGetDto());
            (await _c.GetUserSessionById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((UserSessionGetDto?)null);
            (await _c.GetUserSessionById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteUserSession(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.DeleteUserSession(1)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteUserSessionsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
