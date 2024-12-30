using System;
using AutoMapper;
using MongoDB.Bson;
using Streetcode.BLL.DTO.Users;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Mapping.User;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<DAL.Entities.Users.User, UserDto>()
            .ForMember(ud => ud.Id, conf => conf.MapFrom(u => u.Id.ToString()))
            .ForMember(ud => ud.FullName, conf => conf.MapFrom(u => u.FullName))
            .ForMember(ud => ud.UserName, conf => conf.MapFrom(u => u.UserName))
            .ForMember(ud => ud.Email, conf => conf.MapFrom(u => u.Email))
            .ForMember(ud => ud.PhoneNumber, conf => conf.MapFrom(u => u.PhoneNumber))
            .ForMember(ud => ud.Roles, conf => conf.MapFrom(u => u.Roles))
            .ReverseMap();

        CreateMap<RegistrationDto, DAL.Entities.Users.User>()
            .ForMember(u => u.Id, conf => conf.MapFrom(u => ObjectId.GenerateNewId()))
            .ForMember(u => u.FullName, conf => conf.MapFrom(rd => rd.FullName))
            .ForMember(u => u.UserName, conf => conf.MapFrom(rd => rd.UserName))
            .ForMember(u => u.Email, conf => conf.MapFrom(rd => rd.Email))
            .ForMember(u => u.PhoneNumber, conf => conf.MapFrom(rd => rd.PhoneNumber))
            .ReverseMap();


    }
}