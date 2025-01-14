using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using UserService.BLL.DTO.Users;
using UserService.BLL.Services.Jwt;

namespace UserService.BLL.Interfaces.Authentication;

public interface IGoogleAuthentication
{
    public AuthenticationProperties GoogleLogin();
    public Task<(LoginResultDto Token, IOptions<JwtConfiguration> JwtConfig)> GoogleResponse(AuthenticateResult result);
}