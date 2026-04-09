using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class SecuritySettingMappingProfile:Profile
    {
        public SecuritySettingMappingProfile()
        {
            CreateMap<SecuritySetting, SecuritySettingGetDto>();
            CreateMap<SecuritySettingCreateDto, SecuritySetting>();
            CreateMap<SecuritySetting, SecuritySettingUpdateDto>();
            CreateMap<SecuritySettingUpdateDto, SecuritySetting>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));
        }
    }
}
