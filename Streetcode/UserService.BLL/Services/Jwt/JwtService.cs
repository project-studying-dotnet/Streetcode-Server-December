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

namespace UserService.BLL.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IClaimsService _claimsService;
        private readonly ILogger<JwtService> _logger;

        public JwtService(JwtConfiguration jwtConfiguration, IClaimsService claimsService, ILogger<JwtService> logger)
        {
            _jwtConfiguration = jwtConfiguration;
            _claimsService = claimsService;
            _logger = logger;
        }

        public async Task<string> GenerateTokenAsync(UserEntity user)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var claims = await _claimsService.CreateClaimsAsync(user);

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
    }
}
