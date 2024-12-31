using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateCategoryContent;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.UpdateCategoryContent
{
	public class UpdateCategoryContentHandlerTest
	{
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly UpdateCategoryContentHandler _handler;

		public UpdateCategoryContentHandlerTest()
		{
			_mockMapper = new Mock<IMapper>();
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new UpdateCategoryContentHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenCategoryContentIsUpdated_ShouldReturnsSuccess()
		{
			// Arrange
			var categoryContent = new StreetcodeCategoryContent
			{
				Id = 1,
				Title = "Test Content",
				Text = "Test Description"
			};

			var categoryContentDto = new CategoryContentCreateDto
			{
				Id = 1,
				Title = "Test Content",
				Text = "Test Description"
			};

			var command = new UpdateCategoryContentCommand(categoryContentDto);

			_mockMapper.Setup(m => m.Map<StreetcodeCategoryContent>(categoryContentDto)).Returns(categoryContent);
			_mockMapper.Setup(m => m.Map<CategoryContentCreateDto>(categoryContent)).Returns(categoryContentDto);

			_mockRepositoryWrapper.Setup(rep => rep.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync(categoryContent);

			_mockRepositoryWrapper.Setup(rep => rep.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(categoryContentDto, result.Value);
			_mockRepositoryWrapper.Verify(rep => rep.StreetcodeCategoryContentRepository.Update(categoryContent), Times.Once);
			_mockRepositoryWrapper.Verify(rep => rep.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenCategoryContentIsNull_ShouldReturnsFail()
		{
			// Arrange
			var command = new UpdateCategoryContentCommand(null);
			const string errorMsg = "Source category content no found!";

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal(errorMsg, result.Errors[0].Message);
			_mockLogger.Verify(logger => logger.LogError(command, errorMsg), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenCategoryContentNotFoundInDb_ShouldReturnsFail()
		{
			// Arrange
			var categoryContentDto = new CategoryContentCreateDto
			{
				Id = 1,
				Title = "Test Content",
				Text = "Test Description"
			};

			var command = new UpdateCategoryContentCommand(categoryContentDto);
			const string errorMsg = "Failed to update category content";

			_mockMapper.Setup(m => m.Map<StreetcodeCategoryContent>(categoryContentDto))
				.Returns(new StreetcodeCategoryContent { Id = 1 });
			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

			_mockRepositoryWrapper.Setup(rep => rep.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync((StreetcodeCategoryContent)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal(errorMsg, result.Errors[0].Message);
			_mockLogger.Verify(logger => logger.LogError(command, errorMsg), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenSaveChangesFails_ShouldReturnsFail()
		{
			// Arrange
			var categoryContent = new StreetcodeCategoryContent
			{
				Id = 1,
				Title = "Test Content",
				Text = "Test Description"
			};

			var categoryContentDto = new CategoryContentCreateDto
			{
				Id = 1,
				Title = "Test Content",
				Text = "Test Description"
			};

			var command = new UpdateCategoryContentCommand(categoryContentDto);
			const string errorMsg = "Failed to update category content";

			_mockMapper.Setup(m => m.Map<StreetcodeCategoryContent>(categoryContentDto)).Returns(categoryContent);
			_mockMapper.Setup(m => m.Map<CategoryContentCreateDto>(categoryContent)).Returns(categoryContentDto);

			_mockRepositoryWrapper.Setup(rep => rep.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync(categoryContent);

			_mockRepositoryWrapper.Setup(rep => rep.SaveChangesAsync()).ReturnsAsync(0);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal(errorMsg, result.Errors[0].Message);
			_mockLogger.Verify(logger => logger.LogError(command, errorMsg), Times.Once);
		}
	}
}
