using System.Linq;
using AutoMapper;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Mapping.User;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<DAL.Entities.Users.User, UserDto>()
            .ForMember(ud => ud.Id, conf => conf.MapFrom(u => u.Id))
            .ForMember(ud => ud.FullName, conf => conf.MapFrom(u => u.FullName))
            .ForMember(ud => ud.UserName, conf => conf.MapFrom(u => u.UserName))
            .ForMember(ud => ud.Email, conf => conf.MapFrom(u => u.Email))
            .ForMember(ud => ud.PhoneNumber, conf => conf.MapFrom(u => u.PhoneNumber))
            .ForMember(ud => ud.RoleId, conf => conf.MapFrom(u => u.Roles.FirstOrDefault()));
    }
}