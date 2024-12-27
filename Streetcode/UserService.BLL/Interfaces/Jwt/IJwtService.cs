using System.Threading.Tasks;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Interfaces.Jwt
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(UserEntity user);
        string GenerateRefreshToken();
    }
}
