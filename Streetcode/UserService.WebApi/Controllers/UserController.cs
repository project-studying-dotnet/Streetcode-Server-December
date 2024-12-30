using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.User;
using UserService.DAL.Entities.Users;

namespace UserService.WebApi.Controllers;

[ApiController]
[Route("[action]")]
public class UserController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;

    public UserController(ILoginService loginService, IUserService userService)
    {
        _loginService = loginService;
        _userService = userService;
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

        return Ok(new { token });
    }
}