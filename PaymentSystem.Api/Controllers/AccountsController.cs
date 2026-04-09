using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.AuthDtos;
using PaymentSystem.Shared.ViewModels;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    [AuditLog]
    [ExceptionHandler]
    public class AccountsController : ControllerBase
    {
        readonly IAuthService _authService;
        public AccountsController(IAuthService authService) => _authService = authService;

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");
        }       

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result?.Token == null)
                return Unauthorized("Invalid email or password.");
            return Ok(new { token = result.Token });
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _authService.GoogleLoginAsync(dto);
            if (result?.Token == null)
                return Unauthorized("Login with Google failed.");
            return Ok(new { token = result.Token });
        }

        [HttpPost("login-with-confirm-code")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithConfirmCode([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginWithConfirmCodeAsync(dto);
            if (result == null)
                return Unauthorized("Invalid email or password.");
            if (result.Token == null)
                return Ok(new LoginResponseDto { ConfirmRequired = true });
            return Ok(new LoginResponseDto { Token = result.Token, ConfirmRequired = false });
        }

        [HttpPost("verify-login-confirm-code")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyLoginConfirmCode([FromBody] LoginConfirmCodeDto dto)
        {
            var result = await _authService.VerifyLoginConfirmCodeAsync(dto);
            if (result?.Token == null)
                return BadRequest("Code validation failed.");
            return Ok(new LoginResponseDto { Token = result.Token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result)
                return BadRequest("Registration failed. Email may already be in use.");
            return Ok("Registration successful. Please check your email for confirmation code.");
        }

        [HttpPost("confirm-mail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmMail([FromBody] ConfirmCodeDto dto)
        {
            var result = await _authService.ConfirmMailAsync(dto);
            if (!result)
                return BadRequest("Email verification failed.");
            return Ok("Email confirmed successfully.");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok("If the email exists, a password reset link has been sent.");
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, [FromQuery] string code)
        {
            var result = await _authService.ResetPasswordAsync(dto, code);
            if (!result)
                return BadRequest("Password reset failed. Token may be invalid or expired.");
            return Ok("Password reset successful.");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _authService.ChangePasswordAsync(dto, userId);
            if (!result)
                return BadRequest("Password change failed. Current password may be incorrect.");
            return Ok("Password changed successfully.");
        }

        [HttpGet("get-data-update-profile")]
        [Authorize]
        public async Task<IActionResult> GetDataUpdateProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _authService.GetDataUpdateProfileAsync(userId);
            return Ok(result);
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _authService.UpdateProfileAsync(dto, userId);
            if (!result)
                return BadRequest("Profile update failed.");
            return Ok("Profile updated successfully.");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = GetCurrentUserId();
            var result = await _authService.LogoutAsync(userId);
            if (!result)
                return BadRequest("Logout failed.");
            return Ok("Logged out successfully.");
        }

        [HttpGet("get-role-assign/{id}")]
        [Authorize(Policy = "AllAdmins")]
        public async Task<IActionResult> GetRoleAssign(string id)
        {
            var result = await _authService.GetRoleAssignAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("role-assign/{id}")]
        [Authorize(Policy = "AllAdmins")]
        public async Task<IActionResult> RoleAssign(string id, [FromBody] List<RoleAssignVM> roles)
        {
            var result = await _authService.RoleAssignAsync(roles, id);
            if (!result)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpPost("admin-register")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterDto dto)
        {
            var result = await _authService.AdminRegisterAsync(dto);
            if (!result)
                return BadRequest(MessageConstants.RegisterError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPost("admin-change-password")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AdminChangePassword([FromBody] AdminChangePasswordDto dto)
        {
            var result = await _authService.AdminChangePasswordAsync(dto);
            if (!result)
                return BadRequest(MessageConstants.ChangePasswordError);
            return Ok(MessageConstants.ChangePasswordSuccess);
        }

        [HttpGet("admin-get-profile/{userId}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AdminGetProfile(string userId)
        {
            var result = await _authService.AdminGetProfileAsync(userId);
            if (result == null)
                return NotFound(MessageConstants.NotFound);
            return Ok(result);
        }

        [HttpPut("admin-update-profile")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> AdminUpdateProfile([FromBody] AdminUpdateProfileDto dto)
        {
            var result = await _authService.AdminUpdateProfileAsync(dto);
            if (!result)
                return BadRequest(MessageConstants.UpdateProfileError);
            return Ok(MessageConstants.UpdateProfileSuccess);
        }
    }
}
