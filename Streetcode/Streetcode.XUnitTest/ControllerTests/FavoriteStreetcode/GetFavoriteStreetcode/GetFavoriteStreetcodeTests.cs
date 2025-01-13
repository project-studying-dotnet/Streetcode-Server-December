using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.Interfaces.FavoriteStreetcode;
using Streetcode.WebApi.Controllers.FavoriteStreetcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ControllerTests.FavoriteStreetcode.GetFavoriteStreetcode
{
	public class GetFavoriteStreetcodeTests
	{
		private readonly Mock<ISessionService> _mockSessionService;
		private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
		private readonly FavoriteStreetcodeController _controller;
		private readonly DefaultHttpContext _httpContext;

		public GetFavoriteStreetcodeTests()
		{
			_mockSessionService = new Mock<ISessionService>();
			_mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			_controller = new FavoriteStreetcodeController(_mockSessionService.Object, _mockHttpContextAccessor.Object);

			_httpContext = new DefaultHttpContext();
			_mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);
		}


		[Fact]
		public async Task GetFavorites_SessionNotAvailable_ReturnsInternalServerError()
		{
			// Arrange
			var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };
			var identity = new ClaimsIdentity(claims);
			_httpContext.User = new ClaimsPrincipal(identity);
			_httpContext.Session = null;

			// Act
			var result = await _controller.GetFavorites();

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Session is not available.", statusCodeResult.Value);
		}

		[Fact]
		public async Task GetFavorites_Success_ReturnsStreetcodes()
		{
			// Arrange
			var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };
			var identity = new ClaimsIdentity(claims);
			_httpContext.User = new ClaimsPrincipal(identity);
			_httpContext.Session = new Mock<ISession>().Object;

			var streetcodes = new List<int> { 1, 2, 3 };
			_mockSessionService.Setup(s => s.GetFavoriteStreetcodes(It.IsAny<ISession>(), "user1"))
				.ReturnsAsync(streetcodes);

			// Act
			var result = await _controller.GetFavorites();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(streetcodes, okResult.Value);
		}

		[Fact]
		public async Task GetFavorites_ExceptionThrown_ReturnsInternalServerError()
		{
			// Arrange
			var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };
			var identity = new ClaimsIdentity(claims);
			_httpContext.User = new ClaimsPrincipal(identity);
			_httpContext.Session = new Mock<ISession>().Object;

			_mockSessionService.Setup(s => s.GetFavoriteStreetcodes(It.IsAny<ISession>(), "user1"))
				.ThrowsAsync(new Exception("Test exception"));

			// Act
			var result = await _controller.GetFavorites();

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
			Assert.Equal("Error retrieving favorites: Test exception", statusCodeResult.Value);
		}
	}
}
