using FluentResults;
using System.Security.Claims;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;
using UserService.BLL.DTO.Users;


namespace UserService.BLL.Interfaces.User
{
    public interface ILoginService
    {
        public Task<Result<LoginResultDto>> Login(LoginDto loginDto);
        public Task<Result> Logout(ClaimsPrincipal userPrincipal);
        public Task<Result<LoginResultDto>> RefreshToken(string refreshToken);
    }
}
