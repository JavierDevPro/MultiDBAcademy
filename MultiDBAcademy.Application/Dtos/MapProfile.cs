using AutoMapper;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Dtos;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<User, AuthResponseDto>();
        CreateMap<AuthResponseDto, User>();
        
        CreateMap<User, AuthReguisterResponseDTo>();
        CreateMap<AuthReguisterResponseDTo, User>();
    }
}