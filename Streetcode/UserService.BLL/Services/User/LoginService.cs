using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Streetcode.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Services.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User
{
    public class LoginService : Interfaces.User.ILoginService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginService> _logger;
        private readonly IMapper _mapper;
        private readonly JwtConfiguration _jwtConfiguration;
        public LoginService(UserManager<UserEntity> userManager, IJwtService jwtService, ILogger<LoginService> logger, IMapper mapper, IOptions<JwtConfiguration> options)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;

            _mapper = mapper;
            _jwtConfiguration = options.Value;
        }

        public async Task<Result<LoginResultDTO>> Login(LoginDTO loginDto)
        {
            if (loginDto == null)
            {
                _logger.LogError("Login attempt with null LoginDTO.");
                return Result.Fail("Invalid login request.");
            }

            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogWarning("Invalid login attempt for username: {UserName}", loginDto.UserName);
                return Result.Fail("Invalid login credentials.");
            }

            try
            {
                var accessToken = await _jwtService.GenerateTokenAsync(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenLifetime);

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update user {UserName} with RefreshToken.", loginDto.UserName);
                    return Result.Fail("Failed to save refresh token.");
                }
                _logger.LogInformation("User {UserName} successfully logged in.", loginDto.UserName);
                var result = _mapper.Map<LoginResultDTO>((accessToken, refreshToken));
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {UserName}", loginDto.UserName);
                return Result.Fail("An error occurred while processing your login request.");
            }
        }
    }
}
