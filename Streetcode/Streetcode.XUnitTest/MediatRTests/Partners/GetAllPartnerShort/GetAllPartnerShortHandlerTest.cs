using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Partners.GetAllPartnerShort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.DAL.Entities.Partners;
using Microsoft.EntityFrameworkCore.Query;
using FluentAssertions;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetAllPartnerShort
{
	public class GetAllPartnerShortHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly GetAllPartnerShortHandler _handler;

		public GetAllPartnerShortHandlerTest()
		{
			_mockRepositoryWapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new GetAllPartnerShortHandler(_mockRepositoryWapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoPartnerShortFound_ShouldReturnFail()
		{
			// Arrange
			_mockRepositoryWapper.Setup(repo => repo.PartnersRepository
				.GetAllAsync(null, It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync((List<Partner>)null);

			// Act
			var result = await _handler.Handle(new GetAllPartnersShortQuery(), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenPartnerShortFound_ShouldReturnOk()
		{
			// Arrange
			var partners = new List<Partner>();

			_mockRepositoryWapper.Setup(repo => repo.PartnersRepository
				.GetAllAsync(null, It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync(partners);

			// Act
			var result = await _handler.Handle(new GetAllPartnersShortQuery(), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
		}
	}
}
