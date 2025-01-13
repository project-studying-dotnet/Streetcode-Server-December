using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.Interfaces.User;
using UserEntity = UserService.DAL.Entities.Users.User;


namespace UserService.BLL.Services.User
{
    public class TokenCleanupService : ITokenCleanupService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(UserManager<UserEntity> userManager, ILogger<TokenCleanupService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RemoveExpiredRefreshTokensAsync()
        {
            _logger.LogInformation("Starting cleanup of expired refresh tokens.");

            // Отримання користувачів з простроченими токенами
            var usersWithExpiredTokens = _userManager.Users
                .Where(u => u.RefreshTokens.Any(t => t.RefreshTokenExpiryTime <= DateTime.UtcNow))
                .ToList();

            _logger.LogInformation("Found {UserCount} users with expired tokens.", usersWithExpiredTokens.Count);

            foreach (var user in usersWithExpiredTokens)
            {
                var expiredTokens = user.RefreshTokens
                    .Where(t => t.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    .ToList();

                if (expiredTokens.Any())
                {
                    user.RefreshTokens.RemoveAll(t => expiredTokens.Contains(t));

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError("Failed to update user {UserName} during cleanup.", user.UserName);
                    }
                    else
                    {
                        _logger.LogInformation("Removed {TokenCount} expired tokens for user {UserName}.", expiredTokens.Count, user.UserName);
                    }
                }
            }

            _logger.LogInformation("Completed cleanup of expired refresh tokens.");
        }
    }
}
