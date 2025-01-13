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

namespace Streetcode.XUnitTest.ControllerTests.FavoriteStreetcode.AddFavoriteStreetcode
{
	public class AddFavoriteStreetcodeTest
	{
		private readonly Mock<ISessionService> _mockSessionService;
		private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
		private readonly FavoriteStreetcodeController _controller;
		private readonly DefaultHttpContext _httpContext;

		public AddFavoriteStreetcodeTest()
		{
			_mockSessionService = new Mock<ISessionService>();
			_mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			_controller = new FavoriteStreetcodeController(_mockSessionService.Object, _mockHttpContextAccessor.Object);

			_httpContext = new DefaultHttpContext();
			_mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);
		}


		[Fact]
		public async Task AddFavorite_SessionNotAvailable_ReturnsInternalServerError()
		{
			// Arrange
			var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "userId") };
			_httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
			_httpContext.Session = null;

			// Act
			var result = await _controller.AddFavorite(1);

			// Assert
			var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			Assert.Equal("Session is not available.", internalServerErrorResult.Value);
		}


		[Fact]
		public async Task AddFavorite_ExceptionThrown_ReturnsInternalServerError()
		{
			// Arrange
			var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "userId") };
			_httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
			_httpContext.Session = new Mock<ISession>().Object;

			_mockSessionService.Setup(s => s.AddFavoriteStreetcode(It.IsAny<ISession>(), "userId", 1))
						   .Throws(new Exception("Test exception"));

			// Act
			var result = await _controller.AddFavorite(1);

			// Assert
			var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			Assert.Equal("Error adding favorite: Test exception", internalServerErrorResult.Value);
		}

	}
}
