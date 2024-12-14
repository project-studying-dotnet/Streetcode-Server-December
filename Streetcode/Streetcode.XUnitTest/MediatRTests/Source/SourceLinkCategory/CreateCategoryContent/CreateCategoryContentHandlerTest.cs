using Moq;
using AutoMapper;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces;
using System;
using System.Linq.Expressions;
using Streetcode.BLL.Interfaces.Logging;
using FluentAssertions;
using SourceEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.MediatRTests.Source.SourceLinkCategory.CreateCategoryContent
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
		public async Task Handle_ContentIsNull_ReturnsResultFail()
		{
			// Arrange
			string errMsg = "Cannot create new CategoryContent";

			// Act
			var result = await _handler.Handle(new CreateCategoryContentCommand(null), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(c => c.LogError(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}


		[Fact]
		public async Task Handle_WhenExceptionOccurs_ShouldReturnFail()
		{
			// Arrange
			var command = new CreateCategoryContentCommand(new CategoryContentCreateDTO { Text = "Test Content", SourceLinkCategoryId = 1, StreetcodeId = 1 });
			var contentEntity = new StreetcodeCategoryContent { Text = "Test Content", SourceLinkCategoryId = 1, StreetcodeId = 1 };

			_mockMapper.Setup(m => m.Map<StreetcodeCategoryContent>(command.newContent)).Returns(contentEntity);
			_mockRepositoryWrapper.Setup(r => r.StreetcodeCategoryContentRepository.CreateAsync(contentEntity))
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal("An error occurred while creating the content.", result.Errors[0].Message);
			_mockLogger.Verify(l => l.LogError(It.IsAny<string>(), "Database error"), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenContentCreatedSuccessfully_ShouldReturnSuccess()
		{
			// Arrange
			var newContentDTO = new CategoryContentCreateDTO { Text = "Test Content", SourceLinkCategoryId = 1, StreetcodeId = 1 };
			var contentEntity = new StreetcodeCategoryContent { Text = "Test Content", SourceLinkCategoryId = 1, StreetcodeId = 1 };
			var command = new CreateCategoryContentCommand(newContentDTO);
			var contentDto = new CategoryContentCreateDTO { Text = "Test Content", SourceLinkCategoryId = 1, StreetcodeId = 1 };

			_mockMapper.Setup(m => m.Map<StreetcodeCategoryContent>(command.newContent)).Returns(contentEntity);
			_mockRepositoryWrapper.Setup(r => r.StreetcodeCategoryContentRepository.CreateAsync(contentEntity)).ReturnsAsync(contentEntity);
			_mockRepositoryWrapper.Setup(r => r.SourceCategoryRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<SourceEntity, bool>>>(), null)).ReturnsAsync(new SourceEntity());
			_mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null)).ReturnsAsync(new StreetcodeContent());
			_mockRepositoryWrapper.Setup(r => r.SaveChanges()).Returns(1);
			_mockMapper.Setup(m => m.Map<CategoryContentCreateDTO>(contentEntity)).Returns(contentDto);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().BeEquivalentTo(contentDto);
		}
	}
}
