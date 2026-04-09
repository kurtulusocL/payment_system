using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class WalletMappingProfile:Profile
    {
        public WalletMappingProfile()
        {
            CreateMap<Wallet, WalletGetDto>();
            CreateMap<WalletCreateDto, Wallet>();
            CreateMap<Wallet, WalletUpdateDto>();
            CreateMap<WalletUpdateDto, Wallet>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<Wallet, WalletGetDto>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Transactions != null ? src.Transactions.Count : 0));
        }
    }
}
