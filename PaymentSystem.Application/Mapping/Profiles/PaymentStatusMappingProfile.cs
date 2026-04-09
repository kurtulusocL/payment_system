using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class PaymentStatusMappingProfile:Profile
    {
        public PaymentStatusMappingProfile()
        {
            CreateMap<PaymentStatus, PaymentStatusGetDto>();
            CreateMap<PaymentStatusCreateDto, PaymentStatus>();
            CreateMap<PaymentStatus, PaymentStatusUpdateDto>();
            CreateMap<PaymentStatusUpdateDto, PaymentStatus>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<PaymentStatus, PaymentStatusGetDto>()
                .ForMember(dest => dest.PaymentCount, opt => opt.MapFrom(src => src.Payments != null ? src.Payments.Count : 0));
        }
    }
}
