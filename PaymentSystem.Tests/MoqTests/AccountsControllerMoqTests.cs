using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.AuthDtos;
using PaymentSystem.Shared.ViewModels;
using System.Security.Claims;

namespace PaymentSystem.Tests.MoqTests
{
    public class AccountsControllerMoqTests
    {
        private readonly Mock<IAuthService> _mockService;
        private readonly AccountsController _controller;
        public AccountsControllerMoqTests()
        {
            _mockService = new Mock<IAuthService>();
            _controller = new AccountsController(_mockService.Object);
            SetupUserClaims();
        }

        private void SetupUserClaims()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1")
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            var dto = new LoginDto
            {
                Email = "t@t.com",
                Password = "Pass123!"
            };
            _mockService.Setup(x => x.LoginAsync(dto)).ReturnsAsync(new AuthResponseDto { Token = "jwt" }); var r = await _controller.Login(dto);
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var dto = new LoginDto
            {
                Email = "t@t.com",
                Password = "Wrong"
            };
            _mockService.Setup(x => x.LoginAsync(dto)).ReturnsAsync((AuthResponseDto?)null); var r = await _controller.Login(dto);
            r.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Register_Success_ReturnsOk()
        {
            var dto = new RegisterDto
            {
                NameSurname = "A B",
                Email = "a@a.com",
                Password = "Pass123!",
                Birthdate = DateTime.UtcNow.AddYears(-20)
            };
            _mockService.Setup(x => x.RegisterAsync(dto)).ReturnsAsync(true);
            var r = await _controller.Register(dto); r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_Failure_ReturnsBadRequest()
        {
            var dto = new RegisterDto
            {
                NameSurname = "A B",
                Email = "a@a.com",
                Password = "Pass123!",
                Birthdate = DateTime.UtcNow.AddYears(-20)
            };
            _mockService.Setup(x => x.RegisterAsync(dto)).ReturnsAsync(false);
            var r = await _controller.Register(dto); r.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ForgotPassword_ReturnsOk()
        {
            _mockService.Setup(x => x.ForgotPasswordAsync(It.IsAny<string>())).ReturnsAsync(true);
            var r = await _controller.ForgotPassword(new ForgotPasswordRequestDto
            {
                Email = "t@t.com"
            });
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_Success_ReturnsOk()
        {
            _mockService.Setup(x => x.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(),
                It.IsAny<string>())).ReturnsAsync(true);
            var r = await _controller.ChangePassword(new ChangePasswordDto
            {
                CurrentPassword = "Old1!",
                NewPassword = "New1!",
                ConfirmNewPassword = "New1!"
            });
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Logout_ReturnsOk()
        {
            _mockService.Setup(x => x.LogoutAsync(It.IsAny<string>())).ReturnsAsync(true);
            var r = await _controller.Logout();
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetRoleAssign_ReturnsOk()
        {
            _mockService.Setup(x => x.GetRoleAssignAsync(It.IsAny<string>())).ReturnsAsync(new List<RoleAssignVM>());
            var r = await _controller.GetRoleAssign("1");
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task RoleAssign_Success_ReturnsOk()
        {
            _mockService.Setup(x => x.RoleAssignAsync(It.IsAny<List<RoleAssignVM>>(),
                It.IsAny<string>())).ReturnsAsync(true);
            var r = await _controller.RoleAssign("1", new List<RoleAssignVM>());
            r.Should().BeOfType<OkObjectResult>();
        }

        // Admin-specific tests

        [Fact]
        public async Task AdminRegister_Success_ReturnsOk()
        {
            var dto = new AdminRegisterDto
            {
                NameSurname = "Admin User",
                Email = "admin@t.com",
                UserName = "adminuser",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!",
                Birthdate = DateTime.UtcNow.AddYears(-30)
            };
            _mockService.Setup(x => x.AdminRegisterAsync(dto)).ReturnsAsync(true);
            var r = await _controller.AdminRegister(dto);
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AdminRegister_Failure_ReturnsBadRequest()
        {
            var dto = new AdminRegisterDto
            {
                NameSurname = "Admin User",
                Email = "admin@t.com",
                UserName = "adminuser",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!",
                Birthdate = DateTime.UtcNow.AddYears(-30)
            };
            _mockService.Setup(x => x.AdminRegisterAsync(dto)).ReturnsAsync(false);
            var r = await _controller.AdminRegister(dto);
            r.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AdminChangePassword_Success_ReturnsOk()
        {
            var dto = new AdminChangePasswordDto
            {
                TargetUserId = "user-1",
                NewPassword = "New1!",
                ConfirmNewPassword = "New1!"
            };
            _mockService.Setup(x => x.AdminChangePasswordAsync(dto)).ReturnsAsync(true);
            var r = await _controller.AdminChangePassword(dto);
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AdminChangePassword_Failure_ReturnsBadRequest()
        {
            var dto = new AdminChangePasswordDto
            {
                TargetUserId = "user-1",
                NewPassword = "New1!",
                ConfirmNewPassword = "New1!"
            };
            _mockService.Setup(x => x.AdminChangePasswordAsync(dto)).ReturnsAsync(false);
            var r = await _controller.AdminChangePassword(dto);
            r.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AdminGetProfile_Found_ReturnsOk()
        {
            var resultDto = new AdminUpdateProfileDto
            {
                TargetUserId = "user-1",
                NameSurname = "Test User",
                IsActive = true,
                IsDeleted = false
            };
            _mockService.Setup(x => x.AdminGetProfileAsync("user-1")).ReturnsAsync(resultDto);
            var r = await _controller.AdminGetProfile("user-1");
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AdminGetProfile_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(x => x.AdminGetProfileAsync("user-1")).ReturnsAsync((AdminUpdateProfileDto?)null);
            var r = await _controller.AdminGetProfile("user-1");
            r.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task AdminUpdateProfile_Success_ReturnsOk()
        {
            var dto = new AdminUpdateProfileDto
            {
                TargetUserId = "user-1",
                NameSurname = "Updated User",
                IsActive = true,
                IsDeleted = false
            };
            _mockService.Setup(x => x.AdminUpdateProfileAsync(dto)).ReturnsAsync(true);
            var r = await _controller.AdminUpdateProfile(dto);
            r.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AdminUpdateProfile_Failure_ReturnsBadRequest()
        {
            var dto = new AdminUpdateProfileDto
            {
                TargetUserId = "user-1",
                NameSurname = "Updated User",
                IsActive = true,
                IsDeleted = false
            };
            _mockService.Setup(x => x.AdminUpdateProfileAsync(dto)).ReturnsAsync(false);
            var r = await _controller.AdminUpdateProfile(dto);
            r.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
