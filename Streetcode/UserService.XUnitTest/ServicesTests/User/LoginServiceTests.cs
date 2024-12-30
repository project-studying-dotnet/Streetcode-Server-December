using Moq;
using Xunit;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserService.BLL.DTO.User;
using UserService.BLL.Services.User;
using UserService.BLL.Interfaces.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;
using Streetcode.BLL.DTO.Users;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UserService.BLL.Services.Jwt;
using UserService.BLL.DTO.Users;

namespace UserService.XUnitTest.ServicesTests.User
{
    public class LoginServiceTests
    {
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<LoginService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOptions<JwtConfiguration>> _jwtConfigMock;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _userManagerMock = new Mock<UserManager<UserEntity>>(
                Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<LoginService>>();
            _mapperMock = new Mock<IMapper>();
            _jwtConfigMock = new Mock<IOptions<JwtConfiguration>>();
            _jwtConfigMock.Setup(x => x.Value).Returns(new JwtConfiguration { RefreshTokenLifetime = 7 });

            _loginService = new LoginService(
                _userManagerMock.Object,
                _jwtServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _jwtConfigMock.Object);
        }

        [Fact]
        public async Task Login_Should_Return_Fail_When_LoginDto_Is_Null()
        {
            // Act
            var result = await _loginService.Login(null);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message == "Invalid login request.");
        }

        [Fact]
        public async Task Login_Should_Return_Fail_When_User_Not_Found()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((UserEntity)null);

            var loginDto = new LoginDto { UserName = "invalidUser", Password = "invalidPassword" };

            // Act
            var result = await _loginService.Login(loginDto);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message == "Invalid login credentials.");
        }

        [Fact]
        public async Task Login_Should_Return_Success_With_Tokens_When_Valid_Credentials()
        {
            // Arrange
            var user = new UserEntity { UserName = "validUser" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            _jwtServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync("mockAccessToken");
            _jwtServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("mockRefreshToken");
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var loginDto = new LoginDto { UserName = "validUser", Password = "validPassword" };
            _mapperMock.Setup(m => m.Map<LoginResultDto>(It.IsAny<(string, string)>())).Returns(
                new LoginResultDto { AccessToken = "mockAccessToken", RefreshToken = "mockRefreshToken" });

            // Act
            var result = await _loginService.Login(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("mockAccessToken", result.Value.AccessToken);
            Assert.Equal("mockRefreshToken", result.Value.RefreshToken);
        }

        [Fact]
        public async Task Login_Should_Return_Fail_When_RefreshToken_Not_Saved()
        {
            // Arrange
            var user = new UserEntity { UserName = "validUser" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            _jwtServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync("mockAccessToken");
            _jwtServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("mockRefreshToken");
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed());

            var loginDto = new LoginDto { UserName = "validUser", Password = "validPassword" };

            // Act
            var result = await _loginService.Login(loginDto);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message == "Failed to save refresh token.");
        }
    }
}