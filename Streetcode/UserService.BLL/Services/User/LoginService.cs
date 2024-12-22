using System;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User
{
    public class LoginService : Interfaces.User.ILoginService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginService> _logger;
        public LoginService(UserManager<UserEntity> userManager, IJwtService jwtService, ILogger<LoginService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<Result<string>> Login(LoginDTO loginDto)
        {
            if (loginDto == null)
            {
                _logger.LogError("Login attempt with null LoginDTO.");
                return Result.Fail("Invalid login request.");
            }

            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null && !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogError("Invalid login attempt for username: {UserName}", loginDto.UserName);
                return Result.Fail("Invalid login credentials.");
            }

            try
            {
                var token = await _jwtService.GenerateTokenAsync(user);
                _logger.LogInformation("User {UserName} successfully logged in.", loginDto.UserName);
                return Result.Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {UserName}", loginDto.UserName);
                return Result.Fail("An error occurred while processing your login request.");
            }
        }
    }
}
