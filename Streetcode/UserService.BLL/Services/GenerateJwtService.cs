using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.Interfaces.Jwt;
using UserService.DAL.Entities.Users;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace UserService.BLL.Services
{
    public class GenerateJwtService : IGenerateJwtService
    {
        private readonly UserManager<User> _userManager;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;
        private readonly int _accessTokenLifetime;

        public GenerateJwtService(IConfiguration configuration, UserManager<User> userManager)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            _issuer = "streetcodes.in.ua";
            _audience = "streetcodes.in.ua";
            _secretKey = jwtSettings["Key"];
            _accessTokenLifetime = int.Parse(jwtSettings["AccessTokenLifetime"]);
            _userManager = userManager;
        }
        public async Task<string> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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
