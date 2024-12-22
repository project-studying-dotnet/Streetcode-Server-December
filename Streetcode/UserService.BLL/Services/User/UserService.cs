using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User;

public class UserService(UserManager<UserEntity> userManager) : IUserService
{
    public async Task<UserEntity> Registration(RegistrationDTO registrationDto)
    {
        var user = new UserEntity
        {
            Id = ObjectId.GenerateNewId(),
            UserName = registrationDto.userName,
            FullName = registrationDto.fullName
        };

        var result = await userManager.CreateAsync(user, registrationDto.password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Ошибка при регистрации пользователя: {errors}");
        }

        var registeredUser = await userManager.Users
                                               .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (registeredUser == null)
        {
            throw new Exception("Пользователь не найден после регистрации.");
        }

        return registeredUser;
    }
}