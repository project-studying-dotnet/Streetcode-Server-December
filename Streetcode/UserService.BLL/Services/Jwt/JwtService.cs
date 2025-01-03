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
using UserService.BLL.Interfaces.User;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace UserService.BLL.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IClaimsService _claimsService;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IOptions<JwtConfiguration> options, IClaimsService claimsService, ILogger<JwtService> logger)
        {
            _jwtConfiguration = options.Value;
            _claimsService = claimsService;
            _logger = logger;
        }

        public async Task<string> GenerateTokenAsync(UserEntity user, string sessionId)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            var claims = await _claimsService.CreateClaimsAsync(user, sessionId);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtConfiguration.AccessTokenLifetime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
    }
}
