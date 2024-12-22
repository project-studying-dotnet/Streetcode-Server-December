using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SourceEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.MediatRTests.Source.CreateCategoryContent
{
	public class CreateCategoryContentHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly CreateCategoryContentHandler _handler;

		public CreateCategoryContentHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new CreateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenCreationFails_ShouldReturnFail()
		{
			// Arrange
			var newContent = new CategoryContentCreateDTO
			{
				Text = "Sample Content",
				SourceLinkCategoryId = 1,
				StreetcodeId = 1
			};
			var request = new CreateCategoryContentCommand(newContent);
			var newStreetcodeContent = new StreetcodeCategoryContent
			{
				Text = "Sample Content",
				SourceLinkCategoryId = 1,
				StreetcodeId = 1
			};

			_mockMapper.Setup(mapper => mapper.Map<StreetcodeCategoryContent>(newContent)).Returns(newStreetcodeContent);
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository.CreateAsync(It.IsAny<StreetcodeCategoryContent>())).ReturnsAsync(newStreetcodeContent);
			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<SourceEntity, bool>>>(), null))
				.ReturnsAsync(new SourceEntity { Id = 1, Title = "Category" });
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(new StreetcodeContent { Id = 1, Title = "Streetcode" });
			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Single(result.Errors);
			Assert.Equal("Failed to create a source records", result.Errors.First().Message);

			_mockLogger.Verify(logger => logger.LogError(It.IsAny<CreateCategoryContentCommand>(), "Failed to create a source records"), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeCategoryContentRepository.CreateAsync(It.IsAny<StreetcodeCategoryContent>()), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.SaveChangesAsync(), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.SourceCategoryRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<SourceEntity, bool>>>(), null), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenCreationSucceeds_ShouldReturnSuccess()
		{
			// Arrange
			var newContent = new CategoryContentCreateDTO
			{
				Text = "Sample Content",
				SourceLinkCategoryId = 1,
				StreetcodeId = 1
			};
			var request = new CreateCategoryContentCommand(newContent);
			var newStreetcodeContent = new StreetcodeCategoryContent
			{
				Text = "Sample Content",
				SourceLinkCategoryId = 1,
				StreetcodeId = 1
			};

			_mockMapper.Setup(mapper => mapper.Map<StreetcodeCategoryContent>(newContent)).Returns(newStreetcodeContent);
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository.CreateAsync(It.IsAny<StreetcodeCategoryContent>())).ReturnsAsync(newStreetcodeContent);
			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<SourceEntity, bool>>>(), null))
				.ReturnsAsync(new SourceEntity { Id = 1, Title = "Category" });
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(new StreetcodeContent { Id = 1, Title = "Streetcode" });
			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Empty(result.Errors);

			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeCategoryContentRepository.CreateAsync(It.IsAny<StreetcodeCategoryContent>()), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.SaveChangesAsync(), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.SourceCategoryRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<SourceEntity, bool>>>(), null), Times.Once);
			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null), Times.Once);
		}
	}
}
