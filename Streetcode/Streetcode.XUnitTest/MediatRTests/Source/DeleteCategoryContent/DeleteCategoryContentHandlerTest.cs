using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.DeleteCategoryContent
{
	public class DeleteCategoryContentHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly DeleteCategoryContentHandler _handler;

		public DeleteCategoryContentHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new DeleteCategoryContentHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoSourcesFound_ShouldReturnFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync((StreetcodeCategoryContent)null);

			// Act
			var result = await _handler.Handle(new DeleteCategoryContentCommand(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.First().Message.Should().Be("Cannot find a source with corresponding id: 1");
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Cannot find a source with corresponding id: 1"), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenSourceFound_ShouldReturnSuccess()
		{
			// Arrange
			var source = new StreetcodeCategoryContent { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync(source);

			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
				.ReturnsAsync(1);

			// Act
			var result = await _handler.Handle(new DeleteCategoryContentCommand(1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().Be(Unit.Value);
			_mockRepositoryWrapper.Verify(x => x.StreetcodeCategoryContentRepository.Delete(source), Times.Once);
			_mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenExceptionThrownDuringSave_ShouldReturnFail()
		{
			// Arrange
			var source = new StreetcodeCategoryContent { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync(source);

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository.Delete(source));
			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
				.ReturnsAsync(0);

			// Act
			var result = await _handler.Handle(new DeleteCategoryContentCommand(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.First().Message.Should().Be("Failed to delete a source");
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Failed to delete a source"), Times.Once);
		}
	}

}
