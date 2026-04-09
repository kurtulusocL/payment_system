using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class TransactionTypeMappingProfile:Profile
    {
        public TransactionTypeMappingProfile()
        {
            CreateMap<TransactionType, TransactionTypeGetDto>();
            CreateMap<TransactionTypeCreateDto, TransactionType>();
            CreateMap<TransactionType, TransactionTypeUpdateDto>();
            CreateMap<TransactionTypeUpdateDto, TransactionType>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));

            CreateMap<TransactionType, TransactionTypeGetDto>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Transactions != null ? src.Transactions.Count : 0));
        }
    }
}
