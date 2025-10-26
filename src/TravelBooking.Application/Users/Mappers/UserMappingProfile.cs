using AutoMapper;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Application.Users.DTOs;

namespace TravelBooking.Application.Users.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, LoginResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.TokenType, opt => opt.MapFrom(_ => "Bearer"));
    }
}
