using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using UserService.WebApi.Middleware;
using UserService.BLL.Services.Jwt;
using System.Text;

namespace UserService.XUnitTest.MiddlewareTests
{
    public class CookieMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<CookieMiddleware>> _loggerMock;
        private readonly Mock<IOptions<JwtConfiguration>> _jwtConfigurationMock;
        private readonly CookieMiddleware _middleware;

        public CookieMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<CookieMiddleware>>();
            _jwtConfigurationMock = new Mock<IOptions<JwtConfiguration>>();
            _jwtConfigurationMock.Setup(x => x.Value).Returns(new JwtConfiguration { AccessTokenLifetime = 1 });

            _middleware = new CookieMiddleware(_nextMock.Object, _loggerMock.Object, _jwtConfigurationMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldSkipProcessing_IfTokenPresentInHeader()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer some-token";

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ShouldSkipProcessing_IfTokenPresentInCookie()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.ContainsKey("AuthToken")).Returns(true);
            cookiesMock.Setup(c => c["AuthToken"]).Returns("some-token");

            context.Request.Cookies = cookiesMock.Object;

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ShouldInterceptResponse_AndAddCookie()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Response.ContentType = "application/json";

            var responseJson = "{ \"token\": \"new-token\" }";
            _nextMock.Setup(next => next(context))
                .Callback(() =>
                {
                    var bytes = Encoding.UTF8.GetBytes(responseJson);
                    context.Response.Body.Write(bytes, 0, bytes.Length);
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                })
                .Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Contains("AuthToken", context.Response.Headers.SetCookie.ToString());
        }


        [Fact]
        public async Task InvokeAsync_ShouldNotAppendToken_IfTokenAlreadyExistsInCookie()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.ContainsKey("AuthToken")).Returns(true);
            cookiesMock.Setup(c => c["AuthToken"]).Returns("existing-token");

            context.Request.Cookies = cookiesMock.Object;

            var memoryStream = new MemoryStream();
            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupGet(r => r.Body).Returns(memoryStream);
            responseMock.SetupGet(r => r.ContentType).Returns("application/json");

            // Simulate a response body with the same token
            var responseBody = "{\"token\":\"existing-token\"}";
            var buffer = Encoding.UTF8.GetBytes(responseBody);
            memoryStream.Write(buffer, 0, buffer.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var cookiesMockResponse = new Mock<IResponseCookies>();
            responseMock.SetupGet(r => r.Cookies).Returns(cookiesMockResponse.Object);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(c => c.Response).Returns(responseMock.Object);
            contextMock.Setup(c => c.Request).Returns(context.Request);

            // Act
            await _middleware.InvokeAsync(contextMock.Object);

            // Assert
            cookiesMockResponse.Verify(c => c.Append("AuthToken", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldHandleInvalidJsonGracefully()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var memoryStream = new MemoryStream();

            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupGet(r => r.Body).Returns(memoryStream);
            responseMock.SetupGet(r => r.ContentType).Returns("application/json");

            // Simulate an invalid JSON response body
            var responseBody = "{\"token\":}"; // Invalid JSON
            var buffer = Encoding.UTF8.GetBytes(responseBody);
            memoryStream.Write(buffer, 0, buffer.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var cookiesMock = new Mock<IResponseCookies>();
            responseMock.SetupGet(r => r.Cookies).Returns(cookiesMock.Object);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(c => c.Response).Returns(responseMock.Object);
            contextMock.Setup(c => c.Request).Returns(context.Request);

            // Act
            await _middleware.InvokeAsync(contextMock.Object);

            // Assert
            cookiesMock.Verify(c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never);
        }
    }
}
