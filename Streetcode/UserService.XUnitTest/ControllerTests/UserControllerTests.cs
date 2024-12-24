using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserService.WebApi.Controllers;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace UserService.XUnitTest.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<ILoginService> _loginServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;
        private readonly Mock<HttpContext> _mockHttpContext;

        public UserControllerTests()
        {
            _loginServiceMock = new Mock<ILoginService>();
            _userServiceMock = new Mock<IUserService>();

            _mockHttpContext = SetupMockHttpContext();

            _controller = new UserController(_loginServiceMock.Object, _userServiceMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [Fact]
        public async Task Login_Should_Return_BadRequest_When_Login_Fails()
        {
            // Arrange
            _loginServiceMock.Setup(x => x.Login(It.IsAny<LoginDTO>())).ReturnsAsync(Result.Fail("Invalid credentials"));

            // Act
            var result = await _controller.Login(new LoginDTO());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_Should_Return_Ok_When_Login_Succeeds()
        {
            // Arrange
            _loginServiceMock.Setup(x => x.Login(It.IsAny<LoginDTO>())).ReturnsAsync(Result.Ok("mockToken"));

            // Act
            var result = await _controller.Login(new LoginDTO());

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
