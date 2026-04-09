using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.AppUserDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class AppUserMappingProfile:Profile
    {
        public AppUserMappingProfile()
        {
            CreateMap<AppUser, AppUserGetDto>()
                .ForMember(dest => dest.AuditCount, opt => opt.MapFrom(src => src.Audits != null ? src.Audits.Count : 0))
                .ForMember(dest => dest.PaymentCount, opt => opt.MapFrom(src => src.Payments != null ? src.Payments.Count : 0))
                .ForMember(dest => dest.UserSessionCount, opt => opt.MapFrom(src => src.UserSessions != null ? src.UserSessions.Count : 0));
        }
    }
}
