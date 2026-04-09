using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class ExceptionLoggerMappingProfile : Profile
    {
        public ExceptionLoggerMappingProfile()
        {
            CreateMap<ExceptionLogger, ExceptionLoggerGetDto>();
        }
    }
}