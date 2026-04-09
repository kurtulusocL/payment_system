using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Application.Validators.FluentValidation;
using PaymentSystem.Shared.Dtos.AuthDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace PaymentSystem.Application.Validators.Extention
{
    public static class FluentValidationExtension
    {
        public static void AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginDto>, LoginValidator>();
            services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
            services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordValidator>();
            services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordValidator>();
            services.AddScoped<IValidator<ConfirmCodeDto>, ConfirmCodeValidator>();
            services.AddScoped<IValidator<LoginConfirmCodeDto>, LoginConfirmCodeValidator>();
            services.AddScoped<IValidator<GoogleLoginDto>, GoogleLoginValidator>();
            services.AddScoped<IValidator<UpdateProfileDto>, UpdateProfileValidator>();

            services.AddScoped<IValidator<CurrencyCreateDto>, CurrencyCreateValidator>();
            services.AddScoped<IValidator<CurrencyUpdateDto>, CurrencyUpdateValidator>();

            services.AddScoped<IValidator<MerchantCreateDto>, MerchantCreateValidator>();
            services.AddScoped<IValidator<MerchantUpdateDto>, MerchantUpdateValidator>();

            services.AddScoped<IValidator<MerchantStatusCreateDto>, MerchantStatusCreateValidator>();
            services.AddScoped<IValidator<MerchantStatusUpdateDto>, MerchantStatusUpdateValidator>();
            services.AddScoped<IValidator<PaymentStatusCreateDto>, PaymentStatusCreateValidator>();
            services.AddScoped<IValidator<PaymentStatusUpdateDto>, PaymentStatusUpdateValidator>();
            services.AddScoped<IValidator<TransactionTypeCreateDto>, TransactionTypeCreateValidator>();
            services.AddScoped<IValidator<TransactionTypeUpdateDto>, TransactionTypeUpdateValidator>();

            services.AddScoped<IValidator<PaymentCreateDto>, PaymentCreateValidator>();
            services.AddScoped<IValidator<PaymentUpdateDto>, PaymentUpdateValidator>();

            services.AddScoped<IValidator<TransactionCreateDto>, TransactionCreateValidator>();
            services.AddScoped<IValidator<TransactionUpdateDto>, TransactionUpdateValidator>();

            services.AddScoped<IValidator<WalletCreateDto>, WalletCreateValidator>();
            services.AddScoped<IValidator<WalletUpdateDto>, WalletUpdateValidator>();

            services.AddScoped<IValidator<SecuritySettingCreateDto>, SecuritySettingCreateValidator>();
            services.AddScoped<IValidator<SecuritySettingUpdateDto>, SecuritySettingUpdateValidator>();

            services.AddScoped<IValidator<AppRoleCreateDto>, AppRoleCreateValidator>();
            services.AddScoped<IValidator<AppRoleUpdateDto>, AppRoleUpdateValidator>();

            services.AddScoped<IValidator<UserSessionCreateDto>, UserSessionCreateValidator>();
        }
        
        public static void AddFluentValidationServicesByAssembly(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
