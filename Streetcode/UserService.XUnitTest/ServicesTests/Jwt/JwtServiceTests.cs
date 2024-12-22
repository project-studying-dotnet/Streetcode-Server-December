using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.Services.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.XUnitTest.ServicesTests.Jwt
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<ILogger<JwtService>> _loggerMock;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _userManagerMock = new Mock<UserManager<UserEntity>>(
                Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILogger<JwtService>>();

            _configMock.Setup(c => c.GetSection("Jwt")["SecretKey"]).Returns("VerySecretKey12345678901234567890");
            _configMock.Setup(c => c.GetSection("Jwt")["AccessTokenLifetime"]).Returns("1");

            _jwtService = new JwtService(_configMock.Object, _userManagerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_Should_Throw_If_User_Is_Null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _jwtService.GenerateTokenAsync(null));
        }

        [Fact]
        public async Task GenerateTokenAsync_Should_Return_Token_When_User_Valid()
        {
            // Arrange
            var user = new UserEntity
            { 
                UserName = "testuser",
                Email = "test@example.com",
                FullName = "Test User"
            };

            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var token = await _jwtService.GenerateTokenAsync(user);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.Contains("eyJ", token); // Checking that the token includes the encoded JWT header
        }
    }
}
