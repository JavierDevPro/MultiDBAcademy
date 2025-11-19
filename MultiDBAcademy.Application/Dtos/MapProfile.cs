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
        
        CreateMap<User, RegisterDTo>();
        CreateMap<RegisterDTo, User>()
            .ForMember(dest => dest.PassHash, opt => opt.Ignore())
            // ⬅️ ¡AGREGAR ESTO! Ignora la propiedad de navegación
            .ForMember(dest => dest.Role, opt => opt.Ignore());
    }
}