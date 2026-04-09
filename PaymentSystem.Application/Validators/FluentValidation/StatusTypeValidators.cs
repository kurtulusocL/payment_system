using FluentValidation;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class MerchantStatusCreateValidator : AbstractValidator<MerchantStatusCreateDto>
    {
        public MerchantStatusCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Merchant status name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class MerchantStatusUpdateValidator : AbstractValidator<MerchantStatusUpdateDto>
    {
        public MerchantStatusUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid merchant status ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Merchant status name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class PaymentStatusCreateValidator : AbstractValidator<PaymentStatusCreateDto>
    {
        public PaymentStatusCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Payment status name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class PaymentStatusUpdateValidator : AbstractValidator<PaymentStatusUpdateDto>
    {
        public PaymentStatusUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid payment status ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Payment status name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class TransactionTypeCreateValidator : AbstractValidator<TransactionTypeCreateDto>
    {
        public TransactionTypeCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Transaction type name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class TransactionTypeUpdateValidator : AbstractValidator<TransactionTypeUpdateDto>
    {
        public TransactionTypeUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid transaction type ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Transaction type name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
