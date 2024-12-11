using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetAllTest;

public class GetAllPartnersHandlerTest
{
	private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
	private readonly Mock<IMapper> _mockMapper;
	private readonly Mock<ILoggerService> _mockLogger;
	private readonly GetAllPartnersHandler _handler;

	public GetAllPartnersHandlerTest()
	{
		_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
		_mockMapper = new Mock<IMapper>();
		_mockLogger = new Mock<ILoggerService>();
		_handler = new GetAllPartnersHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
	}


	[Fact]
	public async Task Handle_WhenNoPartnersFound_ShouldReturnFail()
	{
		// Arrange
		_mockRepositoryWrapper
			.Setup(repo => repo.PartnersRepository.GetAllAsync(
				null, // predicate
				It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
				.ReturnsAsync((List<Partner>)null);

		// Act
		var result = await _handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

		// Assert
		result.IsFailed.Should().BeTrue();
		_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task Handle_WhenPartnersFound_ShouldReturnOk()
	{
		// Arrange
		var partners = new List<Partner> { new Partner() };

		_mockRepositoryWrapper.Setup(x => x.PartnersRepository
			.GetAllAsync(
			null,
			It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
			.ReturnsAsync(partners);

		// Act
		var result = await _handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();
	}
}