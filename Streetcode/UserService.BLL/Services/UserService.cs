using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserService.DAL.Entities.Users;

namespace UserService.BLL.Services;

public class UserService(UserManager<User> userManager) : IUserService
{
    public async Task<User> Registration(RegistrationDTO registrationDto)
    {
        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            UserName = registrationDto.userName,
            FullName = registrationDto.fullName
        };

        await userManager.CreateAsync(user, registrationDto.password);

        return await userManager.Users.FirstAsync(u => u.Id == user.Id);
    }
}