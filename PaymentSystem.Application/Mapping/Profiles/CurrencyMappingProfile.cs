using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class CurrencyMappingProfile:Profile
    {
        public CurrencyMappingProfile()
        {
            CreateMap<Currency, CurrencyGetDto>();
            CreateMap<CurrencyCreateDto, Currency>();
            CreateMap<Currency, CurrencyUpdateDto>();
            CreateMap<CurrencyUpdateDto, Currency>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<Currency, CurrencyGetDto>()
                .ForMember(dest => dest.PaymentCount, opt => opt.MapFrom(src => src.Payments != null ? src.Payments.Count : 0))
                .ForMember(dest => dest.WalletCount, opt => opt.MapFrom(src => src.Wallets != null ? src.Wallets.Count : 0))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Transactions != null ? src.Transactions.Count : 0));
        }
    }
}
