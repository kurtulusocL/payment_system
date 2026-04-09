using FluentValidation;
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;

namespace PaymentSystem.Application.Validators.FluentValidation
{
    public class CurrencyCreateValidator : AbstractValidator<CurrencyCreateDto>
    {
        public CurrencyCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Currency name is required.")
                .MaximumLength(100).WithMessage("Currency name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Currency code is required.")
                .MaximumLength(10).WithMessage("Currency code cannot exceed 10 characters.")
                .Matches(@"^[A-Z]{3}$").WithMessage("Currency code must be 3 uppercase letters (e.g., USD, EUR).");

            RuleFor(x => x.Symbol)
                .MaximumLength(10).WithMessage("Symbol cannot exceed 10 characters.")
                .When(x => !string.IsNullOrEmpty(x.Symbol));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class CurrencyUpdateValidator : AbstractValidator<CurrencyUpdateDto>
    {
        public CurrencyUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid currency ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Currency name is required.")
                .MaximumLength(100).WithMessage("Currency name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Currency code is required.")
                .MaximumLength(10).WithMessage("Currency code cannot exceed 10 characters.")
                .Matches(@"^[A-Z]{3}$").WithMessage("Currency code must be 3 uppercase letters (e.g., USD, EUR).");

            RuleFor(x => x.Symbol)
                .MaximumLength(10).WithMessage("Symbol cannot exceed 10 characters.")
                .When(x => !string.IsNullOrEmpty(x.Symbol));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
