using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Interfaces.User;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User
{
    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<EmailConfirmationService> _logger;

        public EmailConfirmationService(UserManager<UserEntity> userManager, IJwtService jwtService, ILogger<EmailConfirmationService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<Result<string>> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                const string errMsg = "UserId or token is null or empty";
                _logger.LogWarning(errMsg);
                return Result.Fail(errMsg);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                const string errMsg = "User not found";
                _logger.LogWarning(errMsg);
                return Result.Fail(errMsg);
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                const string errMsg = "Email confirmation failed";
                _logger.LogWarning(errMsg);
                return Result.Fail(errMsg);
            }

            // JWT token generation after successful email confirmation
            var sessionId = Guid.NewGuid().ToString();
            var tokenResult = await _jwtService.GenerateTokenAsync(user, sessionId);

            return Result.Ok(tokenResult);
        }
    }
}
