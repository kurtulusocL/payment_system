using AutoMapper;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace PaymentSystem.Application.Mapping.Profiles
{
    public class UserSessionMappingProfile:Profile
    {
        public UserSessionMappingProfile()
        {
            CreateMap<UserSession, UserSessionGetDto>();
            CreateMap<UserSessionCreateDto, UserSession>();
        }
    }
}
