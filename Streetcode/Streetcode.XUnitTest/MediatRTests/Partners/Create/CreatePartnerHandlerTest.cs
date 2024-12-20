using AutoMapper;
using FluentAssertions;
using Moq;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.XUnitTest.MediatRTests.Partners.GetPartnersByStreetcodeId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.Create
{
	public class CreatePartnerHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly CreatePartnerHandler _handler;

		public CreatePartnerHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new CreatePartnerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Handler_WhenValidRequest_ShouldReturnSuccess()
		{
			// Arrange
			var newPartner = new Partner { Id = 1, Streetcodes = new List<StreetcodeContent>() };
			var newPartnerDTO = new CreatePartnerDTO { Id = 1 };
			var request = new CreatePartnerQuery(newPartnerDTO);

			var createdPartner = new Partner { Id = 1, Streetcodes = new List<StreetcodeContent>() };

			_mockMapper.Setup(m => m.Map<Partner>(It.IsAny<CreatePartnerDTO>())).Returns(newPartner);
			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository.CreateAsync(It.IsAny<Partner>())).ReturnsAsync(createdPartner);
			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
								  .ReturnsAsync(new List<StreetcodeContent>());

			_mockMapper.Setup(m => m.Map<PartnerDTO>(It.IsAny<Partner>())).Returns(new PartnerDTO { Id = 1 });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().BeEquivalentTo(new PartnerDTO { Id = 1 });
		}

		[Fact]
		public async Task Handler_WhenInvalidRequest_ShouldReturnFails()
		{
			// Arrange
			var newPartner = new Partner { Id = 1, Streetcodes = new List<StreetcodeContent>() };
			var newPartnerDTO = new CreatePartnerDTO { Id = 1 };
			var request = new CreatePartnerQuery(newPartnerDTO);

			_mockMapper.Setup(m => m.Map<Partner>(It.IsAny<CreatePartnerDTO>())).Returns(newPartner);
			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository
				.CreateAsync(It.IsAny<Partner>())).ThrowsAsync(new Exception("Database error"));

			_mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.
				GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
				.ReturnsAsync(new List<StreetcodeContent>());

			_mockMapper.Setup(m => m.Map<PartnerDTO>(It.IsAny<Partner>())).Returns(new PartnerDTO { Id = 1 });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), "Database error"), Times.Once);
		}
	}
}
