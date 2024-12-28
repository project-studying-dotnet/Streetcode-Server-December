using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using System.Security.Claims;
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
    public async Task<ActionResult<UserDTO>> Register([FromBody] RegistrationDTO registrationDto)
    {
        return (await _userService.Registration(registrationDto)).Value;
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

    [HttpPost]
    public async Task<ActionResult> Logout()
    {
        var logoutResult = await _loginService.Logout(User);

        if (logoutResult.IsFailed)
        {
            return BadRequest(logoutResult.Errors);
        }
        Response.Cookies.Delete("AuthToken");
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

        return Ok(refreshResult.Value);
    }

    [HttpGet]
    [Authorize]
    public ActionResult TestEndPoint()
    {
        return Ok("Hello from User Service");
    }
}