using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class MerchantStatusMappingProfile:Profile
    {
        public MerchantStatusMappingProfile()
        {
            CreateMap<MerchantStatus, MerchantStatusGetDto>();
            CreateMap<MerchantStatusCreateDto, MerchantStatus>();
            CreateMap<MerchantStatus, MerchantStatusUpdateDto>();
            CreateMap<MerchantStatusUpdateDto, MerchantStatus>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<MerchantStatus, MerchantStatusGetDto>()
                .ForMember(dest => dest.MerchantCount, opt => opt.MapFrom(src => src.Merchants != null ? src.Merchants.Count : 0));
        }
    }
}
