using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Bcpg;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Partners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetById
{
    public class GetPartnerByIdHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly GetPartnerByIdHandler _handler;

		public GetPartnerByIdHandlerTest()
		{
			_mockRepositoryWapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new GetPartnerByIdHandler(_mockRepositoryWapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handle_WhenNoPartnerFound_ShouldReturnsFail()
		{
			// Arrange
			_mockRepositoryWapper.Setup(x => x.PartnersRepository
				.GetSingleOrDefaultAsync(null, It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync((Partner)null);
			var query = new GetPartnerByIdQuery(1);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenPartnerFound_ShouldReturnsOk()
		{
			// Arrange
			var partner = new Partner { Id = 1 };
			var partnerDto = new PartnerDto { Id = 1 };

			_mockRepositoryWapper.Setup(x => x.PartnersRepository
				.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<Partner, bool>>>(), 
					It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync(partner);

			_mockMapper.Setup(x => x.Map<PartnerDto>(partner)).Returns(partnerDto);

			// Act
			var query = new GetPartnerByIdQuery(1);
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
		}
	}
}
