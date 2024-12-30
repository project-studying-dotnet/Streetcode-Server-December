using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Moq;
using Streetcode.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using UserService.BLL.Services.User;

namespace UserService.XUnitTest.ServicesTests.User
{
    public class RegistrationServiceTest
    {
        private readonly Mock<UserManager<DAL.Entities.Users.User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RegistrationService>> _loggerMock;
        private readonly RegistrationService _registrationService;

        public RegistrationServiceTest()
        {
            _userManagerMock = new Mock<UserManager<DAL.Entities.Users.User>>(
                Mock.Of<IUserStore<DAL.Entities.Users.User>>(), null, null, null, null, null, null, null, null);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RegistrationService>>();
            _registrationService = new RegistrationService(_userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
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

    }
}

