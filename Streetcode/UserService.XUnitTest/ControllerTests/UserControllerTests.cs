using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services.Jwt;
using UserService.WebApi.Controllers;
using UserService.WebApi.Extensions;
using Xunit;

namespace UserService.XUnitTest.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<ILoginService> _loginServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<IOptions<JwtConfiguration>> _jwtConfigurationMock;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _loginServiceMock = new Mock<ILoginService>();
            _userServiceMock = new Mock<IUserService>();
            _jwtConfigurationMock = new Mock<IOptions<JwtConfiguration>>();
            _mockHttpContext = SetupMockHttpContext();

            _jwtConfiguration = new JwtConfiguration { AccessTokenLifetime = 1 }; // Set access token lifetime to 1 hour
            _jwtConfigurationMock.Setup(x => x.Value).Returns(_jwtConfiguration);


            _controller = new UserController(_loginServiceMock.Object, _userServiceMock.Object, _jwtConfigurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
        {
            // Arrange
            _loginServiceMock.Setup(x => x.Login(It.IsAny<LoginDto>())).ReturnsAsync(Result.Fail("Invalid credentials"));

            // Act
            var result = await _controller.Login(new LoginDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var actualErrors = Assert.IsType<List<IError>>(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginSucceeds()
        {
            // Arrange
            _loginServiceMock.Setup(x => x.Login(It.IsAny<LoginDto>())).ReturnsAsync(Result.Ok("mockToken"));

            var expirationTime = DateTime.UtcNow.AddHours(_jwtConfiguration.AccessTokenLifetime);

            // Act
            var result = await _controller.Login(new LoginDto());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        private Mock<HttpContext> SetupMockHttpContext()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();
            var mockRequest = new Mock<HttpRequest>();
            var mockCookies = new Mock<IResponseCookies>();

            mockRequest.Setup(r => r.IsHttps).Returns(true);
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);

            return mockHttpContext;
        }
    }
}
