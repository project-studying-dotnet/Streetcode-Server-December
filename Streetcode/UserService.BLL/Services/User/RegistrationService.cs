using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User;

public class RegistrationService : IUserService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(UserManager<UserEntity> userManager, IMapper mapper, ILogger<RegistrationService> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<UserDto>> Registration(RegistrationDto registrationDto)
    {
        
        if (registrationDto.Password != registrationDto.PasswordConfirm)
        {
            const string errMsg = "Password isn't equal";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }
        
        var user = _mapper.Map<RegistrationDto, UserEntity>(registrationDto);

        if (user is null)
        {
            const string errMsg = "Cannot convert null to user";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        var create = await _userManager.CreateAsync(user, registrationDto.Password);

        if (!create.Succeeded)
        {
            const string errMsg = "Cannot create user";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        var newUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (newUser is null)
        {
            const string errMsg = "Cannot find user in db";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }
        
        var assignRole = await _userManager.AddToRoleAsync(newUser, "User");
        if (!assignRole.Succeeded)
        {
            const string errMsg = "Cannot assign role to user";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }
        
        return Result.Ok(_mapper.Map<UserEntity, UserDto>(user));
    }
}