using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;
using Streetcode.DAL.Entities.Media.Images;
using FluentAssertions;
using Streetcode.DAL.Entities.Sources;
using SourceCategory = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.MediatRTests.Source.Create
{
	public class CreateSourceLinkCategoryHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly CreateSourceLinkCategoryHandler _handler;

		public CreateSourceLinkCategoryHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new CreateSourceLinkCategoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenCategoryIsCreatedSuccessfully_ShouldReturnSuccess()
		{
			// Arrange
			var SourceCategory = new SourceLinkCategoryDTO { Title = "Test Category", ImageId = 1 };
			var command = new CreateSourceLinkCategoryCommand(SourceCategory);

			var image = new Image { Id = 1, BlobName = "test-blob", MimeType = "image/jpeg" };
			var category = new SourceCategory { Id = 1, Title = "Test Category" };
			var categoryDto = new SourceLinkCategoryDTO { Title = "Test Category", Image = new ImageDTO { MimeType = "image/jpeg" } };

			_mockRepositoryWrapper.Setup(repo => repo.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Image, bool>>>(), null))
				.ReturnsAsync(image);
			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository.CreateAsync(It.IsAny<SourceCategory>()))
				.ReturnsAsync(category);
			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

			_mockMapper.Setup(mapper => mapper.Map<SourceCategory>(command.SourceLinkCategory))
				.Returns(category);
			_mockMapper.Setup(mapper => mapper.Map<SourceLinkCategoryDTO>(category))
				.Returns(categoryDto);
			_mockMapper.Setup(mapper => mapper.Map<ImageDTO>(image))
				.Returns(categoryDto.Image);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.Value);
			Assert.Equal("Test Category", result.Value.Title);
		}

		[Fact]
		public async Task Handle_WhenImageNotFound_ShouldReturnFail()
		{
			// Arrange
			var sourceCategory = new SourceLinkCategoryDTO { Title = "Test Category", ImageId = 1 };
			var command = new CreateSourceLinkCategoryCommand(sourceCategory);

			_mockRepositoryWrapper.Setup(repo => repo.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Image, bool>>>(), null))
				.ReturnsAsync((Image)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.NotNull(result.Errors);
			Assert.Contains("Image", result.Errors.First().Message);
		}

		[Fact]
		public async Task Handle_WhenSaveChangesFails_ShouldReturnFail()
		{
			// Arrange
			var sourceCategory = new SourceLinkCategoryDTO { Title = "Test Category", ImageId = 1 };
			var command = new CreateSourceLinkCategoryCommand(sourceCategory);

			var image = new Image { Id = 1, BlobName = "test-blob", MimeType = "image/jpeg" };
			var category = new SourceCategory { Id = 1, Title = "Test Category" };

			_mockRepositoryWrapper.Setup(repo => repo.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Image, bool>>>(), null))
				.ReturnsAsync(image);
			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository.CreateAsync(It.IsAny<SourceCategory>()))
				.ReturnsAsync(category);
			_mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

			_mockMapper.Setup(mapper => mapper.Map<SourceCategory>(command.SourceLinkCategory))
				.Returns(category);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.NotNull(result.Errors);
			Assert.Contains(result.Errors, e => e.Message.Contains("Failed to create a source records"));
		}
	}
}
