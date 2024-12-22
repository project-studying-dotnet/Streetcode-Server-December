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
        var result = await _loginService.Login(loginDto);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        var token = result.Value;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("AuthToken", token, cookieOptions);

        return Ok("Login successful.");
    }
}