using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class RolesControllerMoqTests
    {
        private readonly Mock<IRoleService> _m;
        private readonly RolesController _c;
        public RolesControllerMoqTests()
        {
            _m = new Mock<IRoleService>();
            _c = new RolesController(_m.Object);
        }


        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAll()).Returns(new List<AppRoleGetDto>().AsQueryable());
            _c.GetAllRoles().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllForAdmin()).Returns(new List<AppRoleGetDto>().AsQueryable());
            _c.GetAllRolesForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(new AppRoleGetDto());
            (await _c.GetRoleById("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync("1")).ReturnsAsync((AppRoleGetDto?)null);
            (await _c.GetRoleById("1")).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_ReturnsOk()
        {
            _m.Setup(x => x.GetForEditAsync("1")).ReturnsAsync(new AppRoleUpdateDto());
            (await _c.GetRoleForEdit("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<AppRoleCreateDto>())).Returns(Task.FromResult(Result<AppRoleCreateDto>.Success(new AppRoleCreateDto())));
            (await _c.CreateRole(new AppRoleCreateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<AppRoleUpdateDto>())).Returns(Task.FromResult(Result<AppRoleUpdateDto>.Success(new AppRoleUpdateDto())));
            (await _c.UpdateRole(new AppRoleUpdateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync("1")).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteRole("1")).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<string>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteRolesById(new() { "1" })).Should().BeOfType<OkObjectResult>();
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
