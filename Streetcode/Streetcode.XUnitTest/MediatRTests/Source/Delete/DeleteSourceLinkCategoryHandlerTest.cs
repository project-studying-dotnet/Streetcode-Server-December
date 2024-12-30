using AutoMapper;
using MediatR;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SourceLink = Streetcode.DAL.Entities.Sources.SourceLinkCategory;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.Delete
{
	public class DeleteSourceLinkCategoryHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly DeleteSourceLinkCategoryHandler _handler;

		public DeleteSourceLinkCategoryHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new DeleteSourceLinkCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoSourceCategoryFound_ShouldReturnFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SourceLink, bool>>>(), null))
				.ReturnsAsync((SourceLink)null);

			var command = new DeleteSourceLinkCategoryCommand(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal("Cannot find a source with corresponding id: 1", result.Errors.First().Message);
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Cannot find a source with corresponding id: 1"), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenSourceCategoryFound_ShouldReturnSuccess()
		{
			// Arrange
			var sourceCategory = new SourceLink { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SourceLink, bool>>>(), null))
				.ReturnsAsync(sourceCategory);

			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

			var command = new DeleteSourceLinkCategoryCommand(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(Unit.Value, result.Value);
			_mockRepositoryWrapper.Verify(x => x.SourceCategoryRepository.Delete(sourceCategory), Times.Once);
			_mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenSaveChangesFails_ShouldReturnFail()
		{
			// Arrange
			var sourceCategory = new SourceLink { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SourceLink, bool>>>(), null))
				.ReturnsAsync(sourceCategory);

			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

			var command = new DeleteSourceLinkCategoryCommand(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal("Failed to delete a source", result.Errors.First().Message);
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Failed to delete a source"), Times.Once);
		}
	}
}
