using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class AppRoleMappingProfile:Profile
    {
        public AppRoleMappingProfile()
        {
            CreateMap<AppRole, AppRoleGetDto>();
            CreateMap<AppRoleCreateDto, AppRole>();
            CreateMap<AppRole, AppRoleUpdateDto>();
            CreateMap<AppRoleUpdateDto, AppRole>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));
        }
    }
}
