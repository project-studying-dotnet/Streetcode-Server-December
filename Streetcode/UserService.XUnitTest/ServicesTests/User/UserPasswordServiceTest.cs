using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.Azure;

using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.User;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.XUnitTest.ServicesTests.User
{
    public class UserPasswordServiceTest
    {
        private readonly Mock<UserManager<UserEntity>> _mockUserManager;
        private readonly Mock<IAzureServiceBus> _mockBus;
        private readonly UserPasswordService _service;

        public UserPasswordServiceTest()
        {
            _mockUserManager = MockUserManager();
            _mockBus = new Mock<IAzureServiceBus>();
            _service = new UserPasswordService(_mockUserManager.Object, _mockBus.Object);
        }

        [Fact]

        public async Task ChangePassword_UserDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            string username = "nonexistentUser";
            _mockUserManager.Setup(m => m.FindByNameAsync(username)).ReturnsAsync((UserEntity)null);

            var passChangeDto = new PassChangeDto
            {
                Password = "newPassword",
                PasswordConfirm = "newPassword",
                OldPassword = "oldPassword"
            };

            // Act
            var result = await _service.ChangePassword(passChangeDto, username);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message == "User does not exist!");
        }

        [Fact]
        public async Task ChangePassword_PasswordsDoNotMatch_ShouldReturnFail()
        {
            // Arrange
            string username = "testUser";
            var user = new UserEntity { UserName = username };
            _mockUserManager.Setup(m => m.FindByNameAsync(username)).ReturnsAsync(user);

            var passChangeDto = new PassChangeDto
            {
                Password = "newPassword",
                PasswordConfirm = "differentPassword",
                OldPassword = "oldPassword"
            };

            // Act
            var result = await _service.ChangePassword(passChangeDto, username);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message == "Password and Confirm Password fields do not match.");
        }

        [Fact]
        public async Task ChangePassword_InvalidOldPassword_ShouldReturnFail()
        {
            // Arrange
            string username = "testUser";
            var user = new UserEntity { UserName = username };
            _mockUserManager.Setup(m => m.FindByNameAsync(username)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "oldPassword")).ReturnsAsync(false);

            var passChangeDto = new PassChangeDto
            {
                Password = "newPassword",
                PasswordConfirm = "newPassword",
                OldPassword = "oldPassword"
            };

            // Act
            var result = await _service.ChangePassword(passChangeDto, username);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message == "Invalid Old Password credential.");
        }

        [Fact]
        public async Task ChangePassword_FailedToChangePassword_ShouldReturnFail()
        {
            // Arrange
            string username = "testUser";
            var user = new UserEntity { UserName = username };
            _mockUserManager.Setup(m => m.FindByNameAsync(username)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "oldPassword")).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.ChangePasswordAsync(user, "oldPassword", "newPassword"))
                .ReturnsAsync(IdentityResult.Failed());

            var passChangeDto = new PassChangeDto
            {
                Password = "newPassword",
                PasswordConfirm = "newPassword",
                OldPassword = "oldPassword"
            };

            // Act
            var result = await _service.ChangePassword(passChangeDto, username);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message == "Cannot change password for user");
        }

        [Fact]
        public async Task ChangePassword_Success_ShouldSendEmailAndReturnOk()
        {
            // Arrange
            string username = "testUser";
            var user = new UserEntity { UserName = username, Email = "test@example.com" };
            _mockUserManager.Setup(m => m.FindByNameAsync(username)).ReturnsAsync(user);
            _mockUserManager    .Setup(m => m.CheckPasswordAsync(user, "oldPassword")).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.ChangePasswordAsync(user, "oldPassword", "newPassword"))
                .ReturnsAsync(IdentityResult.Success);

            var passChangeDto = new PassChangeDto
            {
                Password = "newPassword",
                PasswordConfirm = "newPassword",
                OldPassword = "oldPassword"
            };

            // Act
            var result = await _service.ChangePassword(passChangeDto, username);

            // Assert
            Assert.True(result.IsSuccess);

            _mockBus.Verify(bus => bus.SendMessage(
                "emailQueue",
                It.Is<string>(s => s.Contains("test@example.com") && s.Contains("succesfully changed your password"))
            ), Times.Once);
        }

        [Fact]
        public async Task ForgotPassword_UserDoesNotExist_ReturnsFail()
        {
            // Arrange
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((UserEntity)null!);

            // Act
            var result = await _service.ForgotPassword("nonexistent@example.com");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User does not exist!", result.Errors.First().Message);
        }

        [Fact]
        public async Task ForgotPassword_SendsEmailSuccessfully_ReturnsOk()
        {
            // Arrange
            var user = new UserEntity { Email = "user@example.com" };
            var resetToken = "reset-token";

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);

            _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(user))
                            .ReturnsAsync(resetToken);

            _mockBus.Setup(bus => bus.SendMessage(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ForgotPassword(user.Email);

            // Assert
            Assert.True(result.IsSuccess);
            _mockBus.Verify(bus => bus.SendMessage("emailQueue", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_UserDoesNotExist_ReturnsFail()
        {
            // Arrange
            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((UserEntity)null!);

            var passResetDto = new PassResetDto
            {
                Email = "nonexistent@example.com",
                Code = "reset-code",
                Password = "new-password"
            };

            // Act
            var result = await _service.ResetPassword(passResetDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("You do not have Permision!", result.Errors.First().Message);
        }

        [Fact]
        public async Task ResetPassword_InvalidResetToken_ReturnsFail()
        {
            // Arrange
            var user = new UserEntity { Email = "user@example.com" };

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);

            _mockUserManager.Setup(um => um.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed());

            var passResetDto = new PassResetDto
            {
                Email = "user@example.com",
                Code = "invalid-code",
                Password = "new-password"
            };

            // Act
            var result = await _service.ResetPassword(passResetDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("You do not have Permision!", result.Errors.First().Message);
        }

        [Fact]
        public async Task ResetPassword_ValidResetToken_SendsConfirmationEmailAndReturnsOk()
        {
            // Arrange
            var user = new UserEntity { Email = "user@example.com" };

            _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);

            _mockUserManager.Setup(um => um.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            _mockBus.Setup(bus => bus.SendMessage(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            var passResetDto = new PassResetDto
            {
                Email = "user@example.com",
                Code = "valid-code",
                Password = "new-password"
            };

            // Act
            var result = await _service.ResetPassword(passResetDto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockBus.Verify(bus => bus.SendMessage("emailQueue", It.IsAny<string>()), Times.Once);
        }

        private static Mock<UserManager<UserEntity>> MockUserManager()
        {
            var store = new Mock<IUserStore<UserEntity>>();
            return new Mock<UserManager<UserEntity>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }
    }
}
