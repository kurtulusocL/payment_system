using Microsoft.Extensions.DependencyInjection;

namespace PaymentSystem.Application.Mapping
{
    public static class MappingRegistration
    {
        public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingRegistration).Assembly);
            return services;
        }
    }
}
