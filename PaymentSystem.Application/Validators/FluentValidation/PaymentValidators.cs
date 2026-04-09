using FluentValidation;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class PaymentCreateValidator : AbstractValidator<PaymentCreateDto>
    {
        public PaymentCreateValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Payment amount must be greater than 0.")
                .PrecisionScale(18, 2, true).WithMessage("Amount cannot exceed 18 digits with 2 decimal places.");

            RuleFor(x => x.IdempotencyKey)
                .NotEmpty().WithMessage("Idempotency key is required.")
                .MaximumLength(100).WithMessage("Idempotency key cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.MaskedCardNumber)
                .MaximumLength(500).WithMessage("Masked card number cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.MaskedCardNumber));

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .MaximumLength(450).WithMessage("Invalid user ID format.");

            RuleFor(x => x.MerchantId)
                .GreaterThan(0).WithMessage("Valid merchant ID is required.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");

            RuleFor(x => x.PaymentStatusId)
                .GreaterThan(0).WithMessage("Valid payment status ID is required.");
        }
    }

    public class PaymentUpdateValidator : AbstractValidator<PaymentUpdateDto>
    {
        public PaymentUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid payment ID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Payment amount must be greater than 0.")
                .PrecisionScale(18, 2, true).WithMessage("Amount cannot exceed 18 digits with 2 decimal places.");

            RuleFor(x => x.IdempotencyKey)
                .NotEmpty().WithMessage("Idempotency key is required.")
                .MaximumLength(100).WithMessage("Idempotency key cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.MaskedCardNumber)
                .MaximumLength(500).WithMessage("Masked card number cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.MaskedCardNumber));

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .MaximumLength(450).WithMessage("Invalid user ID format.");

            RuleFor(x => x.MerchantId)
                .GreaterThan(0).WithMessage("Valid merchant ID is required.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");

            RuleFor(x => x.PaymentStatusId)
                .GreaterThan(0).WithMessage("Valid payment status ID is required.");
        }
    }
}
