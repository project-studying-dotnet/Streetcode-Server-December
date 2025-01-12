using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Partners;
using Streetcode.Domain.Entities.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Source.GetAll
{
    public class GetAllCategoryNamesHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly GetAllCategoryNamesHandler _handler;

		public GetAllCategoryNamesHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new GetAllCategoryNamesHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenNoCategoryNamesFound_ShouldReturnsFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(r => r.SourceCategoryRepository.GetAllAsync(
				null,
				It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()
			)).ReturnsAsync((IEnumerable<SourceLinkCategory>)null);

			// Act
			var result = await _handler.Handle(new GetAllCategoryNamesQuery(), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenCategoryNamesFound_ShouldReturnsSuccess()
		{
			// Arrange
			var categories = new List<SourceLinkCategory> { new SourceLinkCategory() };

			_mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository
				.GetAllAsync(It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(), null))
				.ReturnsAsync(categories);

			// Act
			var result = await _handler.Handle(new GetAllCategoryNamesQuery(), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
		}
	}
}
