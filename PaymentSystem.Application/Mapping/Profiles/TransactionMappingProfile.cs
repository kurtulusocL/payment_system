using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.EntityDtos.TransactionDtos;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class TransactionMappingProfile:Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionCreateDto, Transaction>();
            CreateMap<Transaction, TransactionUpdateDto>();
            CreateMap<TransactionUpdateDto, Transaction>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));
        }
    }
}
