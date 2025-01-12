using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Services.User;
using UserEntity = UserService.DAL.Entities.Users.User;
using Xunit;

namespace UserService.XUnitTest.ServicesTests.User
{
    public class EmailConfirmationServiceTests
    {
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<EmailConfirmationService>> _loggerMock;
        private readonly EmailConfirmationService _emailConfirmationService;

        public EmailConfirmationServiceTests()
        {
            _userManagerMock = new Mock<UserManager<UserEntity>>(
                Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<EmailConfirmationService>>();

            _emailConfirmationService = new EmailConfirmationService(
                _userManagerMock.Object,
                _jwtServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldFail_WhenUserIdOrTokenIsNullOrEmpty()
        {
            // Arrange
            var userId = "";
            var token = "";

            // Act
            var result = await _emailConfirmationService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("UserId or token is null or empty", result.Errors[0].Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("UserId or token is null or empty")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldFail_WhenUserNotFound()
        {
            // Arrange
            var userId = "123";
            var token = "validToken";

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync((UserEntity)null);

            // Act
            var result = await _emailConfirmationService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("User not found", result.Errors[0].Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldFail_WhenEmailConfirmationFails()
        {
            // Arrange
            var userId = "123";
            var token = "validToken";
            var user = new UserEntity();

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _emailConfirmationService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Email confirmation failed", result.Errors[0].Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email confirmation failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldSucceed_WhenEmailConfirmationSucceeds()
        {
            // Arrange
            var userId = "123";
            var token = "validToken";
            var user = new UserEntity();
            var sessionId = Guid.NewGuid().ToString();
            var jwtToken = "jwtToken";

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            _jwtServiceMock.Setup(j => j.GenerateTokenAsync(user, It.IsAny<string>()))
                .ReturnsAsync(jwtToken);

            // Act
            var result = await _emailConfirmationService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(jwtToken, result.Value);
        }
    } 
}
