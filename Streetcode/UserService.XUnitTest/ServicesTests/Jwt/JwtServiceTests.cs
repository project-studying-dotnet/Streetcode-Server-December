using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.Services.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;
using System.Security.Claims;
using UserService.BLL.Interfaces.User;

namespace UserService.XUnitTest.ServicesTests.Jwt
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<ILogger<JwtService>> _loggerMock;
        private readonly Mock<IClaimsService> _claimsServiceMock;
        private readonly JwtService _jwtService;
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtServiceTests()
        {
            // Mock IConfiguration to return values for Jwt settings
            _configMock = new Mock<IConfiguration>();
            _userManagerMock = new Mock<UserManager<UserEntity>>(
                Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILogger<JwtService>>();
            _claimsServiceMock = new Mock<IClaimsService>(); // Mock IClaimsService

            // Set up Jwt configuration values from the mock configuration
            _configMock.Setup(c => c.GetSection("Jwt")["SecretKey"]).Returns("VerySecretKey12345678901234567890");
            _configMock.Setup(c => c.GetSection("Jwt")["AccessTokenLifetime"]).Returns("1");

            // Create JwtConfiguration from mocked IConfiguration
            _jwtConfiguration = new JwtConfiguration
            {
                SecretKey = _configMock.Object.GetSection("Jwt")["SecretKey"],
                AccessTokenLifetime = int.Parse(_configMock.Object.GetSection("Jwt")["AccessTokenLifetime"])
            };

            // Initialize JwtService with JwtConfiguration and IClaimsService mock
            _jwtService = new JwtService(_jwtConfiguration, _claimsServiceMock.Object, _loggerMock.Object);
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
            var user = new UserEntity { UserName = "testuser", Email = "test@example.com", FullName = "Test User" };

            // Mock the GetRolesAsync call to return a list of roles
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Mock CreateClaimsAsync to return some claims
            _claimsServiceMock.Setup(x => x.CreateClaimsAsync(user)).ReturnsAsync(new List<Claim>
            {
                new Claim("sub", user.UserName),
                new Claim("role", "User")
            });

            // Act
            var token = await _jwtService.GenerateTokenAsync(user);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.Contains("eyJ", token); // Checking that the token includes the encoded JWT header
        }
    }
}
