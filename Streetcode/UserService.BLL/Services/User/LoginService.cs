using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using Streetcode.BLL.DTO.Users;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Services.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;
using UserService.BLL.DTO.Users;
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
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _jwtConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<Result<LoginResultDto>> Login(LoginDto loginDto)
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
                var result = _mapper.Map<LoginResultDto>((accessToken, refreshToken));
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {UserName}", loginDto.UserName);
                return Result.Fail("An error occurred while processing your login request.");
            }
        }

        public async Task<Result> Logout(ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal == null)
            {
                _logger.LogWarning("Logout attempt with null userPrincipal.");
                return Result.Fail("User not authenticated.");
            }

            var userName = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                _logger.LogWarning("Logout attempt with missing NameIdentifier for user.");
                return Result.Fail("User information not found.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning("Logout attempt for non-existing user: {UserName}", userName);
                return Result.Fail("User not found.");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user {UserName} during logout.", userName);
                return Result.Fail("Failed to logout user.");
            }
            _logger.LogInformation("User {UserName} successfully logged out.", userName);
            return Result.Ok();
        }

        public async Task<Result<LoginResultDto>> RefreshToken(string refreshToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
            if (user == null)
            {
                return Result.Fail("Invalid refresh token.");
            }
            var newAccessToken = await _jwtService.GenerateTokenAsync(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenLifetime);
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result.Fail("Failed to update refresh token.");
            }
            var result = _mapper.Map<LoginResultDto>((newAccessToken, newRefreshToken));
            return Result.Ok(result);
        }
    }
}