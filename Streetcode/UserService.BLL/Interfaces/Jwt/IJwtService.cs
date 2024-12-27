using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Interfaces.Jwt
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(UserEntity user);
    }
}
