using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using UserService.BLL.Interfaces.User;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User
{
    public class ClaimsService : IClaimsService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<ClaimsService> _logger;

        public ClaimsService(UserManager<UserEntity> userManager, ILogger<ClaimsService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<Claim>> CreateClaimsAsync(UserEntity user, string sessionId)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("SessionId", sessionId)
            };

            try
            {
                var roles = await _userManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user roles.");
                throw new InvalidOperationException("An error occurred while retrieving user roles.", ex);
            }

            return claims;
        }
    }
}
