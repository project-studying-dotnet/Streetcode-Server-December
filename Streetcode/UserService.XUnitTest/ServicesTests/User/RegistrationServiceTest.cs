using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Moq;
using UserService.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using UserService.BLL.Services.User;
using UserService.BLL.Interfaces.Azure;
using Microsoft.Extensions.Configuration;
using UserService.BLL.DTO.PublishDtos;
using Newtonsoft.Json;

namespace UserService.XUnitTest.ServicesTests.User
{
    public class RegistrationServiceTest
    {
        private readonly Mock<UserManager<DAL.Entities.Users.User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RegistrationService>> _loggerMock;
        private readonly Mock<IAzureServiceBus> _busMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly RegistrationService _registrationService;

        public RegistrationServiceTest()
        {
            _userManagerMock = new Mock<UserManager<DAL.Entities.Users.User>>(
                Mock.Of<IUserStore<DAL.Entities.Users.User>>(), null, null, null, null, null, null, null, null);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RegistrationService>>();
            _busMock = new Mock<IAzureServiceBus>();
            _configMock = new Mock<IConfiguration>();
            _registrationService = new RegistrationService(
                _userManagerMock.Object, _mapperMock.Object,
                _loggerMock.Object,
                _busMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenUserAreCreate()
        {
            // Arrange
            var user = new DAL.Entities.Users.User
            {
                Id = ObjectId.GenerateNewId(),
                FullName = "Test FullName",
            };

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyA*"
            };

            var userDto = new UserDto
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
            };


            _mapperMock.Setup(m => m
                    .Map<RegistrationDto, DAL.Entities.Users.User>(registrationDto))
                .Returns(user);

            _userManagerMock.Setup(um => um
                    .CreateAsync(user, registrationDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<DAL.Entities.Users.User>(), It.IsAny<string>()))
                .Callback<DAL.Entities.Users.User, string>((user, role) =>
                {
                    user.Roles ??= [];
                    user.Roles.Add("User");
                })
                .ReturnsAsync(IdentityResult.Success);

            _mapperMock.Setup(m => m.Map<DAL.Entities.Users.User, UserDto>(user))
                .Returns(userDto);

            var emailToken = "fake-email-token";

            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync(emailToken);

            _configMock.Setup(c => c["Email:ConfirmationUrl"])
                .Returns("https://example.com/confirm");

            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsSuccess);
            Assert.Equal(userDto, res.Value);

        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenUserPasswordDontEqual()
        {
            // Arrange
            const string errMsg = "Password isn't equal";

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyAZ*"
            };

            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsFailed);
            Assert.Equal(errMsg, res.Errors[0].Message);

        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenRegistrationDtoIsNull()
        {
            // Arrange
            const string errMsg = "Cannot convert null to user";

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyA*"
            };


            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsFailed);
            Assert.Equal(errMsg, res.Errors[0].Message);

        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenUserAreNotCreate()
        {
            // Arrange
            const string errMsg = "Cannot create user";

            var user = new DAL.Entities.Users.User
            {
                Id = ObjectId.GenerateNewId(),
                FullName = "Test FullName",
            };

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyA*"
            };

            _mapperMock.Setup(m => m
                    .Map<RegistrationDto, DAL.Entities.Users.User>(registrationDto))
                .Returns(user);

            _userManagerMock.Setup(um => um
                    .CreateAsync(user, registrationDto.Password))
                .ReturnsAsync(IdentityResult.Failed());


            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsFailed);
            Assert.Equal(errMsg, res.Errors[0].Message);

        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenUserNotFound()
        {
            // Arrange
            const string errMsg = "Cannot find user in db";

            var user = new DAL.Entities.Users.User
            {
                Id = ObjectId.GenerateNewId(),
                FullName = "Test FullName",
            };

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyA*"
            };


            _mapperMock.Setup(m => m
                    .Map<RegistrationDto, DAL.Entities.Users.User>(registrationDto))
                .Returns(user);

            _userManagerMock.Setup(um => um
                    .CreateAsync(user, registrationDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((DAL.Entities.Users.User?)null);


            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsFailed);
            Assert.Equal(errMsg, res.Errors[0].Message);

        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenUserAreNotAssignRole()
        {
            // Arrange
            const string errMsg = "Cannot assign role to user";

            var user = new DAL.Entities.Users.User
            {
                Id = ObjectId.GenerateNewId(),
                FullName = "Test FullName",
            };

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyA*"
            };



            _mapperMock.Setup(m => m
                    .Map<RegistrationDto, DAL.Entities.Users.User>(registrationDto))
                .Returns(user);

            _userManagerMock.Setup(um => um
                    .CreateAsync(user, registrationDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<DAL.Entities.Users.User>(), It.IsAny<string>()))
                .Callback<DAL.Entities.Users.User, string>((user, role) =>
                {
                    user.Roles ??= [];
                    user.Roles.Add(role);
                })
                .ReturnsAsync(IdentityResult.Failed());


            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsFailed);
            Assert.Equal(errMsg, res.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnOkResult_WhenUserIsCreatedAndEmailSent()
        {
            // Arrange
            var user = new DAL.Entities.Users.User
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                FullName = "Test FullName",
            };

            var registrationDto = new RegistrationDto
            {
                FullName = "Test FullName",
                Email = "test@example.com",
                Password = "qwertyA*",
                PasswordConfirm = "qwertyA*"
            };

            var userDto = new UserDto
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
            };

            var emailToken = "fake-email-token";

            _mapperMock.Setup(m => m
                .Map<RegistrationDto, DAL.Entities.Users.User>(registrationDto))
                .Returns(user);

            _userManagerMock.Setup(um => um
                .CreateAsync(user, registrationDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<DAL.Entities.Users.User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync(emailToken);

            _configMock.Setup(c => c["Email:ConfirmationUrl"])
                .Returns("https://example.com/confirm");
            _configMock.Setup(c => c["Email:From"])
                .Returns("noreply@yourdomain.com");

            _busMock.Setup(bus => bus.SendMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<DAL.Entities.Users.User, UserDto>(user))
                .Returns(userDto);

            // Act
            var res = await _registrationService.Registration(registrationDto);

            // Assert
            Assert.True(res.IsSuccess);
            Assert.Equal(userDto, res.Value);

            var expectedMessage = JsonConvert.SerializeObject(new EmailMessagePublishDto
            {
                To = registrationDto.Email,
                From = "noreply@yourdomain.com",
                Subject = "Confirm your email",
                Content = $"Please confirm your email by clicking the link: https://example.com/confirm?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}"
            });

            _busMock.Verify(bus => bus.SendMessage("emailQueue", expectedMessage), Times.Once);
        }
    }
}

