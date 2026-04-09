using FluentValidation;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class TransactionCreateValidator : AbstractValidator<TransactionCreateDto>
    {
        public TransactionCreateValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Transaction amount must be greater than 0.")
                .PrecisionScale(18, 2, true).WithMessage("Amount cannot exceed 18 digits with 2 decimal places.");

            RuleFor(x => x.Reference)
                .MaximumLength(200).WithMessage("Reference cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reference));

            RuleFor(x => x.WalletId)
                .GreaterThan(0).WithMessage("Valid wallet ID is required.");

            RuleFor(x => x.PaymentId)
                .GreaterThan(0).When(x => x.PaymentId.HasValue)
                .WithMessage("Valid payment ID is required.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");

            RuleFor(x => x.TransactionTypeId)
                .GreaterThan(0).WithMessage("Valid transaction type ID is required.");
        }
    }

    public class TransactionUpdateValidator : AbstractValidator<TransactionUpdateDto>
    {
        public TransactionUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid transaction ID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Transaction amount must be greater than 0.")
                .PrecisionScale(18, 2, true).WithMessage("Amount cannot exceed 18 digits with 2 decimal places.");

            RuleFor(x => x.Reference)
                .MaximumLength(200).WithMessage("Reference cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reference));

            RuleFor(x => x.WalletId)
                .GreaterThan(0).WithMessage("Valid wallet ID is required.");

            RuleFor(x => x.PaymentId)
                .GreaterThan(0).When(x => x.PaymentId.HasValue)
                .WithMessage("Valid payment ID is required.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");

            RuleFor(x => x.TransactionTypeId)
                .GreaterThan(0).WithMessage("Valid transaction type ID is required.");
        }
    }
}
