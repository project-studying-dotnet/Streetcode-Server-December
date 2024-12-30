    using FluentResults;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Streetcode.BLL.DTO.Users;
    using System.Security.Claims;
    using UserService.BLL.DTO.User;
    using UserService.BLL.Interfaces.User;
    using UserService.BLL.Services.Jwt;
    using UserService.BLL.Services.User;
    using UserService.DAL.Entities.Users;
    using UserService.WebApi.Extensions;

    namespace UserService.WebApi.Controllers;

    [ApiController]
    [Route("[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IOptions<JwtConfiguration> _jwtConfiguration;

        public UserController(ILoginService loginService, IUserService userService,IOptions<JwtConfiguration> jwtConfiguration)
        {
            _loginService = loginService;
            _userService = userService;
            _jwtConfiguration = jwtConfiguration;
        }
    }