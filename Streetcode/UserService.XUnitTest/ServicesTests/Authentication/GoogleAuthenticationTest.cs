using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using UserService.BLL.DTO.User;
using UserService.BLL.DTO.Users;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.Authentication;
using UserService.BLL.Services.Jwt;

namespace UserService.XUnitTest.ServicesTests.Authentication;

public class GoogleAuthenticationTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILoginService> _loginServiceMock;
    private readonly IOptions<JwtConfiguration> _jwtConfig;
    private readonly Mock<ILogger<GoogleAuthentication>> _loggerMock;
    private readonly GoogleAuthentication _googleAuthentication; 

    public GoogleAuthenticationTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loginServiceMock = new Mock<ILoginService>();
        _jwtConfig = Options.Create(new JwtConfiguration());
        _loggerMock = new Mock<ILogger<GoogleAuthentication>>();
        _googleAuthentication = new GoogleAuthentication(_userServiceMock.Object, _loginServiceMock.Object, _jwtConfig,
            _loggerMock.Object);
    }
    [Fact]
    public void GoogleLogin_ReturnsCorrectRedirectUrl()
    {
        // Arrange
        var url = "http://localhost:8002/googleResponse";

        // Act
        var result = _googleAuthentication.GoogleLogin();

        // Assert
        Assert.Equal(url, result.RedirectUri);
    }

    [Fact]
    public async Task GoogleAuth_ReturnOkResult_WhenUserAreLogin()
    {
        // Arrange
        var loginResultDto = new LoginResultDto
        {
            AccessToken = "accessToken",
            RefreshToken = "refreshToken",
        };
        
        
        _loginServiceMock
            .Setup(l => l.Login(It.IsAny<LoginDto>()))
            .ReturnsAsync(loginResultDto);
        
        _userServiceMock
            .Setup(u => u.Registration(It.IsAny<RegistrationDto>()))
            .ReturnsAsync(Result.Fail("fail"));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Mystic User"),
            new Claim("ignoredKey", "IgnoredValue"),
            new Claim("ignoredKey2", "IgnoredValue2"),
            new Claim("ignoredKey3", "IgnoredValue3"),
            new Claim(ClaimTypes.Email, "mystic@realm.com")
        };

        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), "Google");
        var authResult = AuthenticateResult.Success(ticket);
        
        // Act
        var (token, config) = await _googleAuthentication.GoogleResponse(authResult);

        // Assert
        Assert.Equal(loginResultDto, token);
        Assert.Equal(_jwtConfig, config);
    }
    
    [Fact]
    public async Task GoogleAuth_ReturnOkResult_WhenUserAreRegister()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = "1",
            Email = "email@email.com",
        };

        var loginResultDto = new LoginResultDto
        {
            AccessToken = "accessToken",
            RefreshToken = "refreshToken",
        };
        
        _userServiceMock
            .Setup(u => u.Registration(It.IsAny<RegistrationDto>()))
            .ReturnsAsync(userDto);
        
        _loginServiceMock
            .Setup(l => l.Login(It.IsAny<LoginDto>()))
            .ReturnsAsync(loginResultDto);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Mystic User"),
            new Claim("ignoredKey", "IgnoredValue"),
            new Claim("ignoredKey2", "IgnoredValue2"),
            new Claim("ignoredKey3", "IgnoredValue3"),
            new Claim(ClaimTypes.Email, "mystic@realm.com")
        };

        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), "Google");
        var authResult = AuthenticateResult.Success(ticket);
        

        // Act
        var (token, config) = await _googleAuthentication.GoogleResponse(authResult);

        // Assert
        Assert.Equal(loginResultDto, token);
        Assert.Equal(_jwtConfig, config);
    }
    
    [Fact]
    public async Task GoogleAuth_ReturnOkResult_WhenUserAreNotAuth()
    {
        // Arrange
        _userServiceMock
            .Setup(u => u.Registration(It.IsAny<RegistrationDto>()))
            .ReturnsAsync(Result.Fail("fail"));
        
        _loginServiceMock
            .Setup(l => l.Login(It.IsAny<LoginDto>()))
            .ReturnsAsync(Result.Fail("fail"));
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Mystic User"),
            new Claim("ignoredKey", "IgnoredValue"),
            new Claim("ignoredKey2", "IgnoredValue2"),
            new Claim("ignoredKey3", "IgnoredValue3"),
            new Claim(ClaimTypes.Email, "mystic@realm.com")
        };

        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), "Google");
        var authResult = AuthenticateResult.Success(ticket);
        
        // Act
        var (token, config) = await _googleAuthentication.GoogleResponse(authResult);

        // Assert
        Assert.Null(token);
        Assert.Null(config);
    }
}