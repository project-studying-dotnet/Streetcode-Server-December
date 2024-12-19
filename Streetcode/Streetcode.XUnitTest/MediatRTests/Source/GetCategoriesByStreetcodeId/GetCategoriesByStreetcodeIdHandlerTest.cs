using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.GetCategoriesByStreetcodeId
{
	public class GetCategoriesByStreetcodeIdHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly Mock<IBlobService> _mockBlob;
		private readonly GetCategoriesByStreetcodeIdHandler _handler;

		public GetCategoriesByStreetcodeIdHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_mockBlob = new Mock<IBlobService>();
			_handler = new GetCategoriesByStreetcodeIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockBlob.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenNoCategoriesFound_ShouldReturnsFail()
		{
			// Arrange
			var request = new GetCategoriesByStreetcodeIdQuery(1);
			_mockRepositoryWrapper
				.Setup(repo => repo.SourceCategoryRepository.GetAllAsync(
					It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
					It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()))
				.ReturnsAsync((IEnumerable<SourceLinkCategory>)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Single(result.Errors);
			Assert.Equal($"Cant find any source category with the streetcode id {request.StreetcodeId}", result.Errors.First().Message);
		}

		[Fact]
		public async Task Handle_WhenCategoriesByStreetcodeIdFound_ShouldReturnsSuccess()
		{
			// Arrange
			var sourceCategories = new List<SourceLinkCategory>
			{
				new() { Id = 1, Title = "Test Category 1", Image = new Image { BlobName = "image1.png" } },
				new() { Id = 2, Title = "Test Category 2", Image = new Image { BlobName = "image2.png" } }
			};

			var sourceCategoryDTOs = new List<SourceLinkCategoryDTO>
			{
				new() { Id = 1, Title = "Test Category 1", Image = new ImageDTO { BlobName = "image1.png" } },
				new() { Id = 2, Title = "Test Category 2", Image = new ImageDTO { BlobName = "image2.png" } }
			};

			_mockRepositoryWrapper
				.Setup(r => r.SourceCategoryRepository.GetAllAsync(It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(), It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()))
				.ReturnsAsync(sourceCategories);

			_mockMapper.Setup(m => m.Map<IEnumerable<SourceLinkCategoryDTO>>(sourceCategories)).Returns(sourceCategoryDTOs);

			_mockBlob.Setup(b => b.FindFileInStorageAsBase64(It.IsAny<string>()))
					 .Returns<string>(blob => blob == "image1.png" ? "Base64Image1" : "Base64Image2");

			// Act
			var result = await _handler.Handle(new GetCategoriesByStreetcodeIdQuery(1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().BeEquivalentTo(new List<SourceLinkCategoryDTO>
			{
				new() { Id = 1, Title = "Test Category 1", Image = new ImageDTO { BlobName = "image1.png", Base64 = "Base64Image1" } },
				new() { Id = 2, Title = "Test Category 2", Image = new ImageDTO { BlobName = "image2.png", Base64 = "Base64Image2" } }
			});

			_mockRepositoryWrapper.Verify(r => r.SourceCategoryRepository.GetAllAsync(
				It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
				It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()), Times.Once);
			_mockBlob.Verify(b => b.FindFileInStorageAsBase64("image1.png"), Times.Once);
			_mockBlob.Verify(b => b.FindFileInStorageAsBase64("image2.png"), Times.Once);
		}
	}
}
