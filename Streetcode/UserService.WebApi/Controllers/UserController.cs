using Microsoft.AspNetCore.Mvc;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserService.DAL.Entities.Users;

namespace UserService.WebApi.Controllers;

[ApiController]
[Route("[action]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<User>> Register([FromBody] RegistrationDTO registrationDto)
    {
        return await userService.Registration(registrationDto);
    }
}