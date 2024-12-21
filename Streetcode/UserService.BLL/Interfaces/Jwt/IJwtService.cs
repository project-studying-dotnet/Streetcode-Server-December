using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.DAL.Entities.Users;

namespace UserService.BLL.Interfaces.Jwt
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(DAL.Entities.Users.User user);
        public (string Token, DateTime Expiry) GenerateRefreshToken();
    }
}
