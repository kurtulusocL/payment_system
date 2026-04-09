using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class PaymentMappingProfile:Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentGetDto>();
            CreateMap<PaymentCreateDto, Payment>();
            CreateMap<Payment, PaymentUpdateDto>();
            CreateMap<PaymentUpdateDto, Payment>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<Payment, PaymentGetDto>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Transactions != null ? src.Transactions.Count : 0));
        }
    }
}
