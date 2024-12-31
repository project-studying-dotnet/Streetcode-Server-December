using MediatR;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Delete;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Streetcode.Delete
{
	public class DeleteStreetcodeHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly DeleteStreetcodeHandler _handler;

		public DeleteStreetcodeHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new DeleteStreetcodeHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoStreetcodeFound_ShouldReturnFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync((StreetcodeContent)null);

			var command = new DeleteStreetcodeCommand(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal("Cannot find a streetcode with corresponding id: 1", result.Errors.First().Message);
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Cannot find a streetcode with corresponding id: 1"), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenStreetcodeFound_ShouldReturnSuccess()
		{
			// Arrange
			var streetcode = new StreetcodeContent { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(streetcode);

			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

			var command = new DeleteStreetcodeCommand(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(Unit.Value, result.Value);
			_mockRepositoryWrapper.Verify(x => x.StreetcodeRepository.Delete(streetcode), Times.Once);
			_mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenSaveChangesFails_ShouldReturnFail()
		{
			// Arrange
			var streetcode = new StreetcodeContent { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(streetcode);

			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

			var command = new DeleteStreetcodeCommand(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal("Failed to delete a streetcode", result.Errors.First().Message);
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Failed to delete a streetcode"), Times.Once);
		}
	}
}
