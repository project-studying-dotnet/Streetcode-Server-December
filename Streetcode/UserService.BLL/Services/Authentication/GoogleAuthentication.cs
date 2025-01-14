using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.BLL.DTO.User;
using UserService.BLL.DTO.Users;
using UserService.BLL.Interfaces.Authentication;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.Jwt;

namespace UserService.BLL.Services.Authentication;

public class GoogleAuthentication(
    IUserService userService,
    ILoginService loginService,
    IOptions<JwtConfiguration> jwtConfig,
    ILogger<GoogleAuthentication> logger) : IGoogleAuthentication
{
    public AuthenticationProperties GoogleLogin()
    {
        const string redirectUrl = "http://localhost:8002/googleResponse";
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return properties;
    }
    
    public async Task<(LoginResultDto Token, IOptions<JwtConfiguration> JwtConfig)> GoogleResponse(AuthenticateResult result)
    {
        var registrationDto = new RegistrationDto
        {
            FullName = result.Principal.Claims.ElementAt(1).Value,
            UserName = result.Principal.Claims.ElementAt(4).Value,
            Email = result.Principal.Claims.ElementAt(4).Value,
        };

        var loginDto = new LoginDto
        {
            UserName = result.Principal.Claims.ElementAt(4).Value
        };
        
        var userRegisterResult = await userService.Registration(registrationDto);
        
        switch (userRegisterResult.IsSuccess)
        {
            case true:
            {
                var userLoginResult = await loginService.Login(loginDto);
                var token = userLoginResult.Value;
                return (Token:token, JwtConfig:jwtConfig);
            }
            case false:
            {
                logger.LogError(userRegisterResult.Errors.First().Message);
            
                var userLoginResult = await loginService.Login(loginDto);

                if (!userLoginResult.IsSuccess)
                {
                    logger.LogError(userRegisterResult.Errors.First().Message);
                    return (null, null);
                }
                var token = userLoginResult.Value;
                return (Token:token, JwtConfig:jwtConfig);
            }
        }
    }
}