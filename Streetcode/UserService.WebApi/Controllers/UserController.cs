using FluentResults;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<User>> Register([FromBody] RegistrationDTO registrationDto)
    {
        return await _userService.Registration(registrationDto);
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginDTO loginDto)
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