using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.AuditDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class AuditMappingProfile:Profile
    {
        public AuditMappingProfile()
        {
            CreateMap<Audit, AuditGetDto>();
        }
    }
}
