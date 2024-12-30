using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using UserService.BLL.Services.Jwt;
using UserService.WebApi.Extensions;
using Xunit;

namespace UserService.UnitTests.WebApi.Extensions
{
    public class HttpContextExtensionsTests
    {
        private Mock<HttpContext> _mockHttpContext;
        private Mock<HttpResponse> _mockHttpResponse;
        private Mock<IResponseCookies> _mockResponseCookies;
        private Mock<IOptions<JwtConfiguration>> _mockOptions;
        private JwtConfiguration _jwtConfiguration;

        public HttpContextExtensionsTests()
        {
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpResponse = new Mock<HttpResponse>();
            _mockResponseCookies = new Mock<IResponseCookies>();
            _mockOptions = new Mock<IOptions<JwtConfiguration>>();

            _jwtConfiguration = new JwtConfiguration { AccessTokenLifetime = 1 }; // Set access token lifetime to 1 hour
            _mockOptions.Setup(x => x.Value).Returns(_jwtConfiguration);
            _mockHttpContext.Setup(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.Setup(x => x.Cookies).Returns(_mockResponseCookies.Object);
        }

        [Fact]
        public void AppendTokenToCookie_ShouldThrowArgumentNullException_WhenHttpContextIsNull()
        {
            // Arrange
            HttpContext context = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => context.AppendTokenToCookie("token", _mockOptions.Object));
        }

        [Fact]
        public void AppendTokenToCookie_ShouldAppendTokenToCookies()
        {
            // Arrange
            var context = _mockHttpContext.Object;
            var token = "test-token";
            var expirationTime = DateTime.UtcNow.AddHours(_jwtConfiguration.AccessTokenLifetime);

            // Act
            context.AppendTokenToCookie(token, _mockOptions.Object);

            // Assert
            _mockResponseCookies.Verify(cookies => cookies.Append(
                "AuthToken",
                token,
                It.Is<CookieOptions>(options =>
                    options.HttpOnly == true &&
                    options.Secure == true &&
                    options.SameSite == SameSiteMode.None &&
                    // In this test, we check that the Expires parameter is within one second of the expected value. 
                    // This should help avoid discrepancies due to slight differences in time.
                    options.Expires >= expirationTime.AddSeconds(-1) &&
                    options.Expires <= expirationTime.AddSeconds(1)
                )
            ), Times.Once);
        }
    }
}
