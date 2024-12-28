using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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


        public async Task<Result> Logout(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Logout attempt for non-existing user ID: {UserId}", userId);
                return Result.Fail("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user {UserId} during logout.", userId);
                return Result.Fail("Failed to logout user.");
            }

            _logger.LogInformation("User {UserId} successfully logged out.", userId);
            return Result.Ok();
        }
        public async Task<Result<LoginResultDTO>> RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                return Result.Fail("Invalid access token or refresh token.");
            }

            var userName = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Result.Fail("Invalid access token or refresh token.");
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

            var result = _mapper.Map<LoginResultDTO>((newAccessToken, newRefreshToken));
            return Result.Ok(result);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey)),
                ValidateLifetime = false // we want to get the principal from an expired token
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
