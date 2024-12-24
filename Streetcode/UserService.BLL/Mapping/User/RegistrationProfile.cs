using AutoMapper;
using MongoDB.Bson;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Mapping.User;

public class RegistrationProfile : Profile
{
    public RegistrationProfile()
    {
        CreateMap<RegistrationDto, DAL.Entities.Users.User>()
            .ForMember(u => u.FullName, conf => conf.MapFrom(rd => rd.FullName))
            .ForMember(u => u.UserName, conf => conf.MapFrom(rd => rd.UserName))
            .ForMember(u => u.Email, conf => conf.MapFrom(rd => rd.Email))
            .ForMember(u => u.PhoneNumber, conf => conf.MapFrom(rd => rd.PhoneNumber))
            .ReverseMap();
    }
}