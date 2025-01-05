using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Streetcode.BLL.DTO.Users;

using UserService.BLL.DTO.User;
using UserService.BLL.DTO.Users;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.Jwt;

using UserService.WebApi.Extensions;

namespace UserService.WebApi.Controllers;

[ApiController]
[Route("api/[action]")]
public class UserController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IOptions<JwtConfiguration> _jwtConfiguration;


    public UserController(ILoginService loginService, IUserService userService, IOptions<JwtConfiguration> jwtConfiguration)
    {
        _loginService = loginService;
        _userService = userService;
        _jwtConfiguration = jwtConfiguration;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegistrationDto registrationDto)
    {
        return (await _userService.Registration(registrationDto)).Value;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromQuery] LoginDto loginDto)
    {
        var loginResult = await _loginService.Login(loginDto);

        if (loginResult.IsFailed)
        {
            return BadRequest(loginResult.Errors);
        }
        var token = loginResult.Value;
        HttpContext.AppendTokenToCookie(token.AccessToken, _jwtConfiguration);

        return Ok(new { token });
    }

    [HttpPost]
    public async Task<ActionResult> Logout()
    {
        var logoutResult = await _loginService.Logout(User);

        if (logoutResult.IsFailed)
        {
            return BadRequest(logoutResult.Errors);
        }
        HttpContext.DeleteAuthTokenCookie();
        return Ok($"User successfully logged out.");
    }

    [HttpPost]
    public async Task<ActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
    {
        var refreshResult = await _loginService.RefreshToken(tokenRequest.RefreshToken);

        if (refreshResult.IsFailed)
        {
            return BadRequest(refreshResult.Errors);
        }
        var token = refreshResult.Value;
        HttpContext.AppendTokenToCookie(token.AccessToken, _jwtConfiguration);
        return Ok(refreshResult.Value);
    }

}