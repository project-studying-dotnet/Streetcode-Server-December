using Moq;
using Xunit;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.DTO.User;
using UserService.BLL.Services.User;
using UserService.BLL.Interfaces.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.XUnitTest.ServicesTests.User
{
    public class LoginServiceTests
    {
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<LoginService>> _loggerMock;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _userManagerMock = new Mock<UserManager<UserEntity>>(
                Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<LoginService>>();

            _loginService = new LoginService(_userManagerMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Login_Should_Return_Fail_When_LoginDto_Is_Null()
        {
            var result = await _loginService.Login(null);

            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Login_Should_Return_Fail_When_User_Not_Found()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((UserEntity)null!);

            var loginDto = new LoginDTO { UserName = "invalid", Password = "password" };

            // Act
            var result = await _loginService.Login(loginDto);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Login_Should_Return_Success_When_Valid()
        {
            // Arrange
            var user = new UserEntity { UserName = "validuser" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            _jwtServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync("mockToken");

            var loginDto = new LoginDTO { UserName = "validuser", Password = "password" };
            var result = await _loginService.Login(loginDto);

            Assert.True(result.IsSuccess);
            Assert.Equal("mockToken", result.Value);
        }
    }
}
