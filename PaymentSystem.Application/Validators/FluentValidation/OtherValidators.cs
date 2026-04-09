using FluentValidation;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class WalletCreateValidator : AbstractValidator<WalletCreateDto>
    {
        public WalletCreateValidator()
        {
            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Wallet balance cannot be negative.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .MaximumLength(450).WithMessage("Invalid user ID format.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");
        }
    }

    public class WalletUpdateValidator : AbstractValidator<WalletUpdateDto>
    {
        public WalletUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid wallet ID is required.");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Wallet balance cannot be negative.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .MaximumLength(450).WithMessage("Invalid user ID format.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");
        }
    }

    public class SecuritySettingCreateValidator : AbstractValidator<SecuritySettingCreateDto>
    {
        public SecuritySettingCreateValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Security setting type is required.")
                .MaximumLength(100).WithMessage("Type cannot exceed 100 characters.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Security setting value is required.")
                .MaximumLength(1000).WithMessage("Value cannot exceed 1000 characters.");
        }
    }

    public class SecuritySettingUpdateValidator : AbstractValidator<SecuritySettingUpdateDto>
    {
        public SecuritySettingUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid security setting ID is required.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Security setting type is required.")
                .MaximumLength(100).WithMessage("Type cannot exceed 100 characters.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Security setting value is required.")
                .MaximumLength(1000).WithMessage("Value cannot exceed 1000 characters.");
        }
    }

    public class AppRoleCreateValidator : AbstractValidator<AppRoleCreateDto>
    {
        public AppRoleCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(2).WithMessage("Role name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters.");

            RuleFor(x => x.NormalizedName)
                .NotEmpty().WithMessage("Normalized role name is required.")
                .MaximumLength(256).WithMessage("Normalized name cannot exceed 256 characters.");
        }
    }

    public class AppRoleUpdateValidator : AbstractValidator<AppRoleUpdateDto>
    {
        public AppRoleUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Role ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(2).WithMessage("Role name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters.");

            RuleFor(x => x.NormalizedName)
                .NotEmpty().WithMessage("Normalized role name is required.")
                .MaximumLength(256).WithMessage("Normalized name cannot exceed 256 characters.");
        }
    }

    public class UserSessionCreateValidator : AbstractValidator<UserSessionCreateDto>
    {
        public UserSessionCreateValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(150).WithMessage("Username cannot exceed 150 characters.");

            RuleFor(x => x.LoginDate)
                .NotEmpty().WithMessage("Login date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Login date cannot be in the future.");

            RuleFor(x => x.LogoutDate)
                .GreaterThanOrEqualTo(x => x.LoginDate).When(x => x.LogoutDate.HasValue)
                .WithMessage("Logout date must be after login date.");

            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("User ID is required.")
                .MaximumLength(450).WithMessage("Invalid user ID format.");

            RuleFor(x => x.OnlineDurationSeconds)
                .GreaterThanOrEqualTo(0).When(x => x.OnlineDurationSeconds.HasValue)
                .WithMessage("Online duration cannot be negative.");
        }
    }
}
