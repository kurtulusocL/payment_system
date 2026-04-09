using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class MerchantMappingProfile:Profile
    {
        public MerchantMappingProfile()
        {
            CreateMap<Merchant, MerchantGetDto>();
            CreateMap<MerchantCreateDto, Merchant>();
            CreateMap<Merchant, MerchantUpdateDto>();
            CreateMap<MerchantUpdateDto, Merchant>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<Merchant, MerchantGetDto>()
               .ForMember(dest => dest.PaymentCount, opt => opt.MapFrom(src => src.Payments != null ? src.Payments.Count : 0));
        }
    }
}
