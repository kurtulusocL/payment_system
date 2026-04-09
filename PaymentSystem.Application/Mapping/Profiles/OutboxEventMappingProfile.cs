using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.OutboxEventDto;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class OutboxEventMappingProfile:Profile
    {
        public OutboxEventMappingProfile()
        {
            CreateMap<OutboxEvent, OutboxEventGetDto>();
        }
    }
}
