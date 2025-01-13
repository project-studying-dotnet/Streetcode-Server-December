using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Xunit;
using FactEntity = Streetcode.Domain.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.Delete
{
    public class DeleteFactHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly DeleteFactHandler _handler;

		public DeleteFactHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenNoFactFound_ShouldReturnFails()
		{
			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.FactRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), null))
				.ReturnsAsync((FactEntity)null);

			// Act
			var result = await _handler.Handle(new DeleteFactCommand(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(f => f.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenFactFound_ShouldReturnsSuccess()
		{
			// Arrange
			var fact = new FactEntity { Id = 1 };

			_mockRepositoryWrapper.Setup(repo => repo.FactRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), null))
				.ReturnsAsync(fact);

			// Act
			var result = await _handler.Handle(new DeleteFactCommand(1), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			_mockRepositoryWrapper.Verify(f => f.FactRepository.Delete(fact));
			_mockRepositoryWrapper.Verify(f => f.SaveChanges(), Times.Once);
		}

		[Fact]
		public async Task Handler_WhenExceptionThrownDuringSave_ShouldReturnFail()
		{
			var errMsg = "Fact removal failed";
			var fact = new FactEntity { Id = 1 };

			// Arrange
			_mockRepositoryWrapper.Setup(repo => repo.FactRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), null))
				.ReturnsAsync(fact);

			_mockRepositoryWrapper.Setup(repo => repo.FactRepository.Delete(fact));
			_mockRepositoryWrapper.Setup(repo => repo.SaveChanges()).Throws(new Exception(errMsg));

			// Act
			var result = await _handler.Handle(new DeleteFactCommand(1), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors[0].Message.Should().Be(errMsg);
			_mockLogger.Verify(f => f.LogError(It.IsAny<object>(), errMsg), Times.Once);
		}
	}
}
