using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.DAL.Entities.Users;

namespace UserService.BLL.Interfaces.Jwt
{
    public interface IGenerateJwtService
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
