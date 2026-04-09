using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.AppUserDtos;
using PaymentSystem.Shared.Results;
namespace PaymentSystem.Tests.MoqTests
{
    public class UsersControllerMoqTests
    {
        private readonly Mock<IUserService> _m;
        private readonly UsersController _c;
        public UsersControllerMoqTests()
        {
            _m = new Mock<IUserService>();
            _c = new UsersController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<AppUserGetDto>().AsQueryable());
            _c.GetAllUsers().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetInactive_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingDeActiveUser()).Returns(new List<AppUserGetDto>().AsQueryable());
            _c.GetAllInactiveUsers().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetDeleted_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingDeletedUser()).Returns(new List<AppUserGetDto>().AsQueryable());
            _c.GetAllDeletedUsers().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetActive_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingActiveUser()).Returns(new List<AppUserGetDto>().AsQueryable());
            _c.GetAllActiveUsers().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<AppUserGetDto>().AsQueryable());
            _c.GetAllUsersForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(new AppUserGetDto());
            (await _c.GetUserById("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync("1")).ReturnsAsync((AppUserGetDto?)null);
            (await _c.GetUserById("1")).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync("1")).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteUser("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<string>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteUsersById(new() { "1" })).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk()
        {
            _m.Setup(x => x.SetActiveAsync("1")).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SetActive("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInactive_ReturnsOk()
        {
            _m.Setup(x => x.SetInActiveAsync("1")).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SetInactive("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SoftDelete_ReturnsOk()
        {
            _m.Setup(x => x.SetDeletedAsync("1")).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SoftDelete("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Restore_ReturnsOk()
        {
            _m.Setup(x => x.SetNotDeletedAsync("1")).ReturnsAsync(Result<bool>.Success(true));
            (await _c.Restore("1")).Should().BeOfType<OkObjectResult>();
        }
    }
}
