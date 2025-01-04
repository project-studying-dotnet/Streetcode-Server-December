using Moq;
using Xunit;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.WebApi.Controllers;
using UserService.BLL.DTO.User;
using UserService.BLL.Services.Jwt;
using UserService.BLL.Interfaces.User;
using Streetcode.BLL.DTO.Users;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UserService.BLL.DTO.Users;

namespace UserService.XUnitTest.ControllersTests;

public class UserControllerTests
{
    private readonly Mock<ILoginService> _loginServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IOptions<JwtConfiguration>> _jwtConfigurationMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _loginServiceMock = new Mock<ILoginService>();
        _userServiceMock = new Mock<IUserService>();
        _jwtConfigurationMock = new Mock<IOptions<JwtConfiguration>>();
        _jwtConfigurationMock.Setup(x => x.Value).Returns(new JwtConfiguration { RefreshTokenLifetime = 7 });

        _userController = new UserController(
            _loginServiceMock.Object,
            _userServiceMock.Object,
            _jwtConfigurationMock.Object
        );

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Register_Should_Return_UserDto_When_Successful()
    {
        // Arrange
        var registrationDto = new RegistrationDto { FullName = "Test User", Password = "Password123!", PasswordConfirm = "Password123!" };
        var userDto = new UserDto { Id = "123", FullName = "Test User" };

        _userServiceMock.Setup(s => s.Registration(registrationDto)).ReturnsAsync(Result.Ok(userDto));

        // Act
        var result = await _userController.Register(registrationDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
        var returnValue = Assert.IsType<UserDto>(actionResult.Value);
        Assert.Equal(userDto, returnValue);
    }

    [Fact]
    public async Task Login_Should_Return_BadRequest_When_Login_Fails()
    {
        // Arrange
        var loginDto = new LoginDto { UserName = "TestUser", Password = "WrongPassword" };

        _loginServiceMock.Setup(s => s.Login(loginDto)).ReturnsAsync(Result.Fail(new List<IError> { new Error("Invalid login") }));

        // Act
        var result = await _userController.Login(loginDto);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IError>>(actionResult.Value);
        Assert.Single(errors);
        Assert.Equal("Invalid login", errors.First().Message);
    }


    [Fact]
    public async Task Logout_Should_Return_BadRequest_When_Logout_Fails()
    {
        // Arrange
        _loginServiceMock.Setup(s => s.Logout(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(Result.Fail(new List<IError> { new Error("Logout failed") }));

        // Act
        var result = await _userController.Logout();

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IError>>(actionResult.Value);
        Assert.Single(errors);
        Assert.Equal("Logout failed", errors.First().Message);
    }

    [Fact]
    public async Task Logout_Should_Return_Ok_When_Successful()
    {
        // Arrange
        _loginServiceMock.Setup(s => s.Logout(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(Result.Ok());

        // Act
        var result = await _userController.Logout();

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User successfully logged out.", actionResult.Value);
    }

    [Fact]
    public async Task RefreshToken_Should_Return_BadRequest_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var tokenRequest = new TokenRequestDTO { RefreshToken = "invalid_token" };

        _loginServiceMock.Setup(s => s.RefreshToken(tokenRequest.RefreshToken)).ReturnsAsync(Result.Fail(new List<IError> { new Error("Invalid refresh token") }));

        // Act
        var result = await _userController.RefreshToken(tokenRequest);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IError>>(actionResult.Value);
        Assert.Single(errors);
        Assert.Equal("Invalid refresh token", errors.First().Message);
    }

    [Fact]
    public async Task RefreshToken_Should_Return_Token_When_Successful()
    {
        // Arrange
        var tokenRequest = new TokenRequestDTO { RefreshToken = "valid_token" };
        var tokenDto = new LoginResultDto { AccessToken = "new_access_token", RefreshToken = "new_refresh_token" };

        _loginServiceMock.Setup(s => s.RefreshToken(tokenRequest.RefreshToken)).ReturnsAsync(Result.Ok(tokenDto));

        // Act
        var result = await _userController.RefreshToken(tokenRequest);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<LoginResultDto>(actionResult.Value);
        Assert.Equal(tokenDto, returnValue);
    }
}
