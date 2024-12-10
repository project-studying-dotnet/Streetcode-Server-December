﻿using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Update;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.Update
{
	public class UpdatePartnerHandlerTest
	{
		private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILoggerService> _mockLogger;
		private readonly UpdatePartnerHandler _handler;

		public UpdatePartnerHandlerTest()
		{
			_mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILoggerService>();
			_handler = new UpdatePartnerHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
		}


		[Fact]
		public async Task Handler_WhenNoPartnerFound_ShouldReturnsFail()
		{
			// Arrange
			var newPartnerDTO = new CreatePartnerDTO { Id = 1 };
			var request = new UpdatePartnerQuery(newPartnerDTO);

			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository
				.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Partner, bool>>>(), null))
				.ReturnsAsync((Partner)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Handle_WhenPartnerUpdated_ShouldReturnSuccess()
		{
			// Arrange
			var updatePartnerDTO = new CreatePartnerDTO { Id = 1, Streetcodes = new List<StreetcodeShortDTO> { new StreetcodeShortDTO { Id = 1 } } }; // Використовуємо CreatePartnerDTO з правильними типами
			var partnerDTO = new PartnerDTO { Id = 1 };
			var partner = new Partner { Id = 1, Streetcodes = new List<StreetcodeContent> { new StreetcodeContent { Id = 1 } } };
			var updatedPartner = new Partner { Id = 1, Streetcodes = new List<StreetcodeContent> { new StreetcodeContent { Id = 1 } } };

			_mockMapper.Setup(m => m.Map<Partner>(updatePartnerDTO)).Returns(partner);
			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository.Update(partner));
			_mockRepositoryWrapper.Setup(repo => repo.SaveChanges()).Returns(1);
			_mockRepositoryWrapper.Setup(repo => repo.PartnerSourceLinkRepository
				.GetAllAsync(It.IsAny<Expression<Func<PartnerSourceLink, bool>>>(), null))
				.ReturnsAsync(new List<PartnerSourceLink>());

			_mockRepositoryWrapper.Setup(repo => repo.PartnerStreetcodeRepository
				.GetAllAsync(It.IsAny<Expression<Func<StreetcodePartner, bool>>>(), null))
				.ReturnsAsync(new List<StreetcodePartner>());

			_mockMapper.Setup(m => m.Map<PartnerDTO>(partner)).Returns(partnerDTO);

			// Act
			var result = await _handler.Handle(new UpdatePartnerQuery(updatePartnerDTO), CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().BeEquivalentTo(partnerDTO);
		}

		[Fact]
		public async Task Handle_WhenExceptionThrown_ShouldReturnFail()
		{
			var errorMsg = "Saving updates";
			var updatePartnerDTO = new CreatePartnerDTO { Id = 1, Streetcodes = new List<StreetcodeShortDTO> { new StreetcodeShortDTO { Id = 1 } } };
			var partner = new Partner { Id = 1, Streetcodes = new List<StreetcodeContent> { new StreetcodeContent { Id = 1 } } };

			// Arrange
			_mockMapper.Setup(m => m.Map<Partner>(updatePartnerDTO)).Returns(partner);
			_mockRepositoryWrapper.Setup(repo => repo.PartnersRepository.Update(It.IsAny<Partner>()));
			_mockRepositoryWrapper.Setup(repo => repo.SaveChanges()).Throws(new Exception(errorMsg));

			_mockRepositoryWrapper.Setup(repo => repo.PartnerSourceLinkRepository
				.GetAllAsync(It.IsAny<Expression<Func<PartnerSourceLink, bool>>>(), null))
				.ReturnsAsync(new List<PartnerSourceLink>());

			_mockRepositoryWrapper.Setup(repo => repo.PartnerStreetcodeRepository
				.GetAllAsync(It.IsAny<Expression<Func<StreetcodePartner, bool>>>(), null))
				.ReturnsAsync(new List<StreetcodePartner>());

			// Act
			var result = await _handler.Handle(new UpdatePartnerQuery(updatePartnerDTO), CancellationToken.None);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.First().Message.Should().Be(errorMsg);
			_mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errorMsg), Times.Once);
		}

	}
}