using FluentResults;
using Streetcode.BLL.DTO.Users;
using System.Security.Claims;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Interfaces.User
{
    public interface ILoginService
    {
        public Task<Result<LoginResultDTO>> Login(LoginDTO loginDto);
        public Task<Result> Logout(ClaimsPrincipal userPrincipal);
        Task<Result<LoginResultDTO>> RefreshToken(string refreshToken);
    }
}
