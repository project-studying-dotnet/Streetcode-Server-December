using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserService.DAL.Entities.Users;

namespace UserService.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, IMapper mapper, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<UserDto>> Registration(RegistrationDto registrationDto)
    {
        var user = _mapper.Map<RegistrationDto, User>(registrationDto);
        if (user is null)
        {
            const string errMsg = "Cannot convert registration dto to user entity";
            _logger.LogError(errMsg);
            return Result.Fail(new Error(errMsg));
        }
        
        var userCreate = await _userManager.CreateAsync(user, registrationDto.Password);

        if (!userCreate.Succeeded)
        {
            const string errMsg = "Cannot create user";
            _logger.LogError(errMsg);
            return Result.Fail(new Error(errMsg));
        }
        
        var newUser = await _userManager.Users.FirstAsync(u => u.Id == user.Id);
        await _userManager.AddToRoleAsync(newUser, registrationDto.Role);
        return Result.Ok(_mapper.Map<User, UserDto>(newUser));

    }
}