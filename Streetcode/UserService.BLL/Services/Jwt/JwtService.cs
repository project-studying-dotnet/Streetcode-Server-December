using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.Interfaces.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger _logger;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;
        private readonly int _accessTokenLifetime;

        public JwtService(IConfiguration configuration, UserManager<UserEntity> userManager, ILogger logger)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            _issuer = "streetcodes.in.ua";
            _audience = "streetcodes.in.ua";
            _secretKey = jwtSettings["SecretKey"];
            _accessTokenLifetime = int.Parse(jwtSettings["AccessTokenLifetime"]);
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<string> GenerateTokenAsync(UserEntity user)
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_accessTokenLifetime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
