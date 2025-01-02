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
using System.Linq;
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
                var accessToken = await _jwtService.GenerateTokenAsync(user, newSessionId);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var currentSessionId = Guid.NewGuid().ToString();
                var refreshTokenInfo  = new RefreshTokenInfo
                {
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenLifetime),
                    SessionId = currentSessionId
                };
                user.RefreshTokens.Add(refreshTokenInfo);
                user.RefreshTokens = user.RefreshTokens.Where(t => t.RefreshTokenExpiryTime > DateTime.UtcNow).ToList();
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
            var sessionId = userPrincipal.FindFirstValue("SessionId");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(sessionId))
            {
                _logger.LogWarning("Logout attempt with missing Name or SessionId for user.");
                return Result.Fail("User information not found.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning("Logout attempt for non-existing user: {UserName}", userName);
                return Result.Fail("User not found.");
            }
            var sessionToRemove = user.RefreshTokens.SingleOrDefault(t => t.SessionId == sessionId);
            if (sessionToRemove != null)
            {
                user.RefreshTokens.Remove(sessionToRemove);
            }
            user.RefreshTokens = user.RefreshTokens.Where(t => t.RefreshTokenExpiryTime > DateTime.UtcNow).ToList();
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
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.RefreshToken == refreshToken && t.RefreshTokenExpiryTime > DateTime.UtcNow));

            if (user == null)
            {
                _logger.LogWarning("Invalid refresh token attempt.");
                return Result.Fail("Invalid refresh token.");
            }

            var refreshTokenInfo = user.RefreshTokens.SingleOrDefault(t => t.RefreshToken == refreshToken);
            if (refreshTokenInfo == null)
            {
                _logger.LogWarning("Refresh token not found for user {UserName}.", user.UserName);
                return Result.Fail("Refresh token not found.");
            }

            var newAccessToken = await _jwtService.GenerateTokenAsync(user, refreshTokenInfo.SessionId);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            refreshTokenInfo.RefreshToken = newRefreshToken;
            refreshTokenInfo.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenLifetime);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update refresh token for user {UserName}.", user.UserName);
                return Result.Fail("Failed to update refresh token.");
            }

            _logger.LogInformation("Successfully refreshed token for user {UserName}.", user.UserName);
            var result = _mapper.Map<LoginResultDto>((newAccessToken, newRefreshToken));
            return Result.Ok(result);
        }
    }
}