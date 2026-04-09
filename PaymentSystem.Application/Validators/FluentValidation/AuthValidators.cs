using FluentValidation;
using PaymentSystem.Shared.Dtos.AuthDtos;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }

    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.NameSurname)
                .NotEmpty().WithMessage("Name/Surname is required.")
                .MinimumLength(3).WithMessage("Name/Surname must be at least 3 characters.")
                .MaximumLength(150).WithMessage("Name/Surname cannot exceed 150 characters.")
                .Matches(@"^[\w\s\-\.]+$").WithMessage("Name/Surname contains invalid characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\-\+\(\)\s]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number format is invalid.")
                .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Birthdate)
                .NotEmpty().WithMessage("Birthdate is required.")
                .LessThan(DateTime.UtcNow).WithMessage("Birthdate must be in the past.")
                .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Invalid birthdate.");
        }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.ResetToken)
                .NotEmpty().WithMessage("Reset token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }
    }

    public class ConfirmCodeValidator : AbstractValidator<ConfirmCodeDto>
    {
        public ConfirmCodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.ConfirmCode)
                .NotEmpty().WithMessage("Confirmation code is required.")
                .Matches(@"^\d{6}$").WithMessage("Confirmation code must be 6 digits.");
        }
    }

    public class LoginConfirmCodeValidator : AbstractValidator<LoginConfirmCodeDto>
    {
        public LoginConfirmCodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.LoginConfirmCode)
                .InclusiveBetween(100000, 999999).WithMessage("Confirmation code must be 6 digits.");
        }
    }

    public class GoogleLoginValidator : AbstractValidator<GoogleLoginDto>
    {
        public GoogleLoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.ProviderKey)
                .NotEmpty().WithMessage("Provider key is required.");

            RuleFor(x => x.NameSurname)
                .MaximumLength(150).WithMessage("Name/Surname cannot exceed 150 characters.")
                .When(x => !string.IsNullOrEmpty(x.NameSurname));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\-\+\(\)\s]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number format is invalid.")
                .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Title));
        }
    }

    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.NameSurname)
                .NotEmpty().WithMessage("Name/Surname is required.")
                .MinimumLength(3).WithMessage("Name/Surname must be at least 3 characters.")
                .MaximumLength(150).WithMessage("Name/Surname cannot exceed 150 characters.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\-\+\(\)\s]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number format is invalid.")
                .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.");

            RuleFor(x => x.Birthdate)
                .NotEmpty().WithMessage("Birthdate is required.")
                .LessThan(DateTime.UtcNow).WithMessage("Birthdate must be in the past.")
                .GreaterThan(DateTime.UtcNow.AddYears(-150)).WithMessage("Invalid birthdate.");
        }
    }
}
