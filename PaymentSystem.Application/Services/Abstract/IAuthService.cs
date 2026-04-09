using PaymentSystem.Shared.Dtos.AuthDtos;
using PaymentSystem.Shared.ViewModels;

namespace PaymentSystem.Application.Services.Abstract
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto login);
        Task<AuthResponseDto?> LoginWithConfirmCodeAsync(LoginDto login);
        Task<AuthResponseDto?> VerifyLoginConfirmCodeAsync(LoginConfirmCodeDto model);
        Task<AuthResponseDto?> GoogleLoginAsync(GoogleLoginDto dto);
        Task<bool> RegisterAsync(RegisterDto model);
        Task<bool> ConfirmMailAsync(ConfirmCodeDto model);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model, string userId);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model, string code);
        Task<UpdateProfileDto> GetDataUpdateProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(UpdateProfileDto model, string userId);
        Task<List<RoleAssignVM>> GetRoleAssignAsync(string id);
        Task<bool> RoleAssignAsync(List<RoleAssignVM> modelList, string id);
        Task<bool> LogoutAsync(string userId);
        Task<bool> ForgotPasswordAsync(string email);

        // Admin-specific authentication methods
        Task<bool> AdminRegisterAsync(AdminRegisterDto model);
        Task<bool> AdminChangePasswordAsync(AdminChangePasswordDto model);
        Task<AdminUpdateProfileDto> AdminGetProfileAsync(string targetUserId);
        Task<bool> AdminUpdateProfileAsync(AdminUpdateProfileDto model);
    }
}
