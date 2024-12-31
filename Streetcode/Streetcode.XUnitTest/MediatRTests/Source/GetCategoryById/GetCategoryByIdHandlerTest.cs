using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.XUnitTest.MediatRTests.Source.GetCategoryById
{
	public class GetCategoryByIdHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly Mock<IBlobService> _mockBlob;
		private readonly GetCategoryByIdHandler _handler;

		public GetCategoryByIdHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_mockBlob = new Mock<IBlobService>();
			_handler = new GetCategoryByIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockBlob.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenCategoriesNoFound_ShouldReturnsFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(), null))
				.ReturnsAsync((SourceLinkCategory)null);

			// Act
			var result = await _handler.Handle(new GetCategoryByIdQuery(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenCategoryFound_ShouldReturnSuccess()
		{
			// Arrange
			var sourceCategory = new SourceLinkCategory
			{
				Id = 1,
				Title = "Test Category",
				Image = new Image { BlobName = "image1.png" }
			};

			var categoryDto = new SourceLinkCategoryDto
			{
				Id = 1,
				Title = "Test Category",
				Image = new ImageDto { BlobName = "image1.png", Base64 = "Base64Image1" }
			};

			_mockRepositoryWrapper
				.Setup(repo => repo.SourceCategoryRepository.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
					It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()))
				.ReturnsAsync(sourceCategory);

			_mockMapper.Setup(m => m.Map<SourceLinkCategoryDto>(sourceCategory)).Returns(categoryDto);
			_mockBlob.Setup(service => service.FindFileInStorageAsBase64("image1.png")).Returns("Base64Image1");

			// Act
			var result = await _handler.Handle(new GetCategoryByIdQuery(1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Title.Should().Be("Test Category");
			result.Value.Image.Base64.Should().Be("Base64Image1");

			_mockRepositoryWrapper.Verify(repo => repo.SourceCategoryRepository.GetFirstOrDefaultAsync(
				It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
				It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()), Times.Once);

			_mockBlob.Verify(b => b.FindFileInStorageAsBase64("image1.png"), Times.Once);
		}
	}
}
