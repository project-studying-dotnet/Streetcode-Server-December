using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.BLL.Attributes;
using UserService.BLL.DTO.User;
using UserService.BLL.DTO.Users;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.Jwt;
using UserService.BLL.Services.User;
using UserService.DAL.Entities.Users;
using UserService.DAL.Enums;
using UserService.WebApi.Extensions;

namespace UserService.WebApi.Controllers;

[ApiController]
[Route("[action]")]
public class UserController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private IOptions<JwtConfiguration> _jwtConfiguration;

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
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        var loginResult = await _loginService.Login(loginDto);

        if (loginResult.IsFailed)
        {
            return BadRequest(loginResult.Errors);
        }

        var token = loginResult.Value;

        HttpContext.AppendTokenToCookie(token, _jwtConfiguration);

        return Ok(new { token });
    }
}