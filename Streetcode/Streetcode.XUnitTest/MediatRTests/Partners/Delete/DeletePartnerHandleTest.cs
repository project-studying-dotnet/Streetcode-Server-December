using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.Delete
{
	public class DeletePartnerHandleTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly DeletePartnerHandler _handler;

		public DeletePartnerHandleTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new DeletePartnerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoPartnerFound_ShouldReturnsFail()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Partner, bool>>>(), null))
				.ReturnsAsync((Partner)null);

			// Act
			var result = await _handler.Handle(new DeletePartnerQuery(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenPartnerFound_ShouldReturnsSuccess()
		{
			// Arrange
			var partner = new Partner { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Partner, bool>>>(), null))
				.ReturnsAsync(partner);

			// Act
			var result = await _handler.Handle(new DeletePartnerQuery(1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			_mockRepositoryWrapper.Verify(x => x.PartnersRepository.Delete(partner));
			_mockRepositoryWrapper.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenExceptionThrownDuringSave_ShouldReturnFail()
		{
			var errMsg = "Partner removal failed";

			// Arrange
			var partner = new Partner { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Partner, bool>>>(), null))
				.ReturnsAsync(partner);

			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository.Delete(partner));
			_mockRepositoryWrapper.Setup(repo => repo.SaveChanges()).Throws(new Exception(errMsg));

			// Act
			var result = await _handler.Handle(new DeletePartnerQuery(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors[0].Message.Should().Be(errMsg);
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errMsg), Times.Once);
		}
	}
}
