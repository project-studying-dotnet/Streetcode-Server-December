using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.Services.Jwt;
using UserEntity = UserService.DAL.Entities.Users.User;
using System.Security.Claims;
using UserService.BLL.Interfaces.User;
using Microsoft.Extensions.Options;

namespace UserService.XUnitTest.ServicesTests.Jwt
{
    public class JwtServiceTests
    {
        private readonly Mock<IOptions<JwtConfiguration>> _optionsMock;
        private readonly Mock<ILogger<JwtService>> _loggerMock;
        private readonly Mock<IClaimsService> _claimsServiceMock;
        private readonly JwtService _jwtService;
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtServiceTests()
        {
            _jwtConfiguration = new JwtConfiguration
            {
                SecretKey = "VerySecretKey12345678901234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenLifetime = 1
            };

            _optionsMock = new Mock<IOptions<JwtConfiguration>>();
            _optionsMock.Setup(o => o.Value).Returns(_jwtConfiguration);

            _loggerMock = new Mock<ILogger<JwtService>>();

            _claimsServiceMock = new Mock<IClaimsService>();

            _jwtService = new JwtService(_optionsMock.Object, _claimsServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_Should_Throw_If_User_Is_Null()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _jwtService.GenerateTokenAsync(null,null));
        }

        [Fact]
        public async Task GenerateTokenAsync_Should_Return_Token_When_User_Valid()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            var user = new UserEntity { UserName = "testuser", Email = "test@example.com", FullName = "Test User" };

            _claimsServiceMock.Setup(x => x.CreateClaimsAsync(user, sessionId)).ReturnsAsync(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "User")
            });

            // Act
            var token = await _jwtService.GenerateTokenAsync(user, sessionId);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.Contains("eyJ", token); // Verify the token structure (JWT header)
        }
    }
}
