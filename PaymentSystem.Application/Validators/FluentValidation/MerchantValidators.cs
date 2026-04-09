using FluentValidation;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class MerchantCreateValidator : AbstractValidator<MerchantCreateDto>
    {
        public MerchantCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Merchant name is required.")
                .MinimumLength(2).WithMessage("Merchant name must be at least 2 characters.")
                .MaximumLength(200).WithMessage("Merchant name cannot exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\-\+\(\)\s]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number format is invalid.")
                .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.");

            RuleFor(x => x.TaxNumber)
                .MaximumLength(50).WithMessage("Tax number cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.TaxNumber));

            RuleFor(x => x.MerchantStatusId)
                .GreaterThan(0).WithMessage("Valid merchant status ID is required.");
        }
    }

    public class MerchantUpdateValidator : AbstractValidator<MerchantUpdateDto>
    {
        public MerchantUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid merchant ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Merchant name is required.")
                .MinimumLength(2).WithMessage("Merchant name must be at least 2 characters.")
                .MaximumLength(200).WithMessage("Merchant name cannot exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\-\+\(\)\s]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number format is invalid.")
                .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.");

            RuleFor(x => x.TaxNumber)
                .MaximumLength(50).WithMessage("Tax number cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.TaxNumber));

            RuleFor(x => x.MerchantStatusId)
                .GreaterThan(0).WithMessage("Valid merchant status ID is required.");
        }
    }
}
