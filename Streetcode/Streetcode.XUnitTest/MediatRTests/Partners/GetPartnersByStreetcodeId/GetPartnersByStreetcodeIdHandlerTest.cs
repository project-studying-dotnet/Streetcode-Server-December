using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetPartnersByStreetcodeId
{
	public class GetPartnersByStreetcodeIdHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly GetPartnersByStreetcodeIdHandler _handler;

		public GetPartnersByStreetcodeIdHandlerTest()
		{
			_mockRepositoryWapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new GetPartnersByStreetcodeIdHandler(_mockMapper.Object, _mockRepositoryWapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoStreetcodeFound_ShouldReturnsFail()
		{
			// Arrange
			_mockRepositoryWapper.Setup(repo => repo.StreetcodeRepository
				.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync((StreetcodeContent)null);

			// Act
			var query = new GetPartnersByStreetcodeIdQuery(1);
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenNoPartnersFound_ShouldReturnsFail()
		{
			// Arrange
			var streetcode = new StreetcodeContent { Id = 1 };

			_mockRepositoryWapper.Setup(repo => repo.StreetcodeRepository
				.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(streetcode);

			_mockRepositoryWapper.Setup(repo => repo.PartnersRepository
				.GetAllAsync(It.IsAny<Expression<Func<Partner, bool>>>(), It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync((IEnumerable<Partner>)null);

			// Act
			var result = await _handler.Handle(new GetPartnersByStreetcodeIdQuery(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenExceptionThrown_ShouldReturnsOK()
		{
			// Arrange
			var streetcode = new StreetcodeContent { Id = 1 };
			var partners = new List<Partner> { };

			_mockRepositoryWapper.Setup(repo => repo.StreetcodeRepository
				.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(streetcode);

			_mockRepositoryWapper.Setup(repo => repo.PartnersRepository
				.GetAllAsync(It.IsAny<Expression<Func<Partner, bool>>>(), It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync(partners);

			// Act
			var result = await _handler.Handle(new GetPartnersByStreetcodeIdQuery(1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
		}
	}
}
