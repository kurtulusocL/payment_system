using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class SecuritySettingsControllerMoqTests
    {
        private readonly Mock<ISecuritySettingService> _m;
        private readonly SecuritySettingsController _c;

        public SecuritySettingsControllerMoqTests()
        {
            _m = new Mock<ISecuritySettingService>();
            _c = new SecuritySettingsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAll()).Returns(new List<SecuritySettingGetDto>().AsQueryable());
            _c.GetAllSecuritySettings().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllForAdmin()).Returns(new List<SecuritySettingGetDto>().AsQueryable());
            _c.GetAllSecuritySettingsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new SecuritySettingGetDto());
            (await _c.GetSecuritySettingById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((SecuritySettingGetDto?)null);
            (await _c.GetSecuritySettingById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync(new SecuritySettingGetDto());
            (await _c.GetSecuritySettingForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync((SecuritySettingGetDto?)null);
            (await _c.GetSecuritySettingForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<SecuritySettingCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.CreateSecuritySetting(new SecuritySettingCreateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<SecuritySettingCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.CreateSecuritySetting(new SecuritySettingCreateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<SecuritySettingUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.UpdateSecuritySetting(new SecuritySettingUpdateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<SecuritySettingUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.UpdateSecuritySetting(new SecuritySettingUpdateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteSecuritySetting(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteSecuritySettingsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
