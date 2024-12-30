using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.GetCategoryContentByStreetcodeId
{
	public class GetCategoryContentByStreetcodeIdHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly GetCategoryContentByStreetcodeIdHandler _handler;

		public GetCategoryContentByStreetcodeIdHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new GetCategoryContentByStreetcodeIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenNoCategoryContentFound_ShouldReturnsFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
					null))
				.ReturnsAsync(new StreetcodeContent { Id = 1 });

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null))
				.ReturnsAsync((StreetcodeCategoryContent)null);

			// Act
			var result = await _handler.Handle(new GetCategoryContentByStreetcodeIdQuery(1, 1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.Should().ContainSingle();
			result.Errors[0].Message.Should().Be("Cannot find any streetcode content");

			_mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), "Cannot find any streetcode content"), Times.Once);

			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
					null), Times.Once);

			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(), null), Times.Once);
		}


		[Fact]
		public async Task Handle_WhenCategoryContentFound_ShouldReturnSuccess()
		{
			// Arrange
			var streetcodeContent = new StreetcodeContent { Id = 1 };
			var streetcodeCategoryContent = new StreetcodeCategoryContent
			{
				StreetcodeId = 1,
				SourceLinkCategoryId = 1,
				Text = "Sample content text"
			};

			var expectedDto = new StreetcodeCategoryContentDto
			{
				StreetcodeId = 1,
				SourceLinkCategoryId = 1,
				Text = "Sample content text"
			};

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
					null))
				.ReturnsAsync(streetcodeContent);

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(),
					null))
				.ReturnsAsync(streetcodeCategoryContent);

			_mockMapper.Setup(mapper => mapper.Map<StreetcodeCategoryContentDto>(streetcodeCategoryContent))
				.Returns(expectedDto);

			// Act
			var result = await _handler.Handle(new GetCategoryContentByStreetcodeIdQuery(1, 1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().NotBeNull();
			result.Value.Should().BeEquivalentTo(expectedDto);

			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeRepository
				.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
					null), Times.Once);

			_mockRepositoryWrapper.Verify(repo => repo.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(),
					null), Times.Once);

			_mockMapper.Verify(mapper => mapper.Map<StreetcodeCategoryContentDto>(streetcodeCategoryContent), Times.Once);
		}

	}
}
