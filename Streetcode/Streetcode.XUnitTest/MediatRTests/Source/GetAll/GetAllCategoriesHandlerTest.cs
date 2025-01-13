using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Sources;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.GetAll
{
    public class GetAllCategoriesHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly Mock<IBlobService> _mockBlob;
		private readonly GetAllCategoriesHandler _handler;

		public GetAllCategoriesHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_mockBlob = new Mock<IBlobService>();
			_handler = new GetAllCategoriesHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockBlob.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenNoSourceCategoriesFound_ShouldReturnsFail()
		{
			// Arrange
			var query = new GetAllCategoriesQuery();
			const string errorMsg = "Cannot find any categories";

			_mockRepositoryWrapper.Setup(r => r.SourceCategoryRepository.GetAllAsync(
				null,
				It.IsAny<List<string>>()
			)).ReturnsAsync((IEnumerable<SourceLinkCategory>)null!);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.Should().ContainSingle(e => e.Message == errorMsg);

			_mockLogger.Verify(
				l => l.LogError(It.IsAny<object>(), errorMsg),
				Times.Once
			);

			_mockRepositoryWrapper.Verify(
				r => r.SourceCategoryRepository.GetAllAsync(
					null,
					It.IsAny<List<string>>()
				),
				Times.Once
			);
		}

		[Fact]
		public async Task Handle_WhenSourceCategoryFound_ShouldReturnsSuccess()
		{
			// Arrange
			var query = new GetAllCategoriesQuery();
			var categories = new List<SourceLinkCategory> { new SourceLinkCategory() };

			_mockRepositoryWrapper.Setup(r => r.SourceCategoryRepository.GetAllAsync(
				null,
				It.IsAny<List<string>>()
			)).ReturnsAsync(categories);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
		}
	}
}
