using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class CreatePositionHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreatePositionHandler _createPositionHandler;

        public CreatePositionHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new PositionProfile())).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _createPositionHandler = new CreatePositionHandler(_mapper, _mockRepositoryWrapper.Object, _mockLogger.Object);

            Positions position = new Positions
            {
                Id = 1,
                Position = "Posit1",
            };

            _mockRepositoryWrapper.Setup(p => p.PositionRepository.CreateAsync(It.IsAny<Positions>()).Result).Returns(position);
        }

        [Fact]
        public async Task Handle_PositionCreatedSuccessfully_ReturnsResultOK()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(p => p.SaveChanges()).Returns(1);
            var request = new CreatePositionQuery(new PositionDTO { Id = 1, Position = "Posit1" });

            // Act
            var result = await _createPositionHandler.Handle(request, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_SaveChangesFail_ReturnsResultFail()
        {
            // Arrange
            const string errorMsg = $"SaveChanges Error";
            _mockRepositoryWrapper.Setup(p => p.SaveChanges()).Throws(new Exception(errorMsg));
            var request = new CreatePositionQuery(new PositionDTO { Id = 1, Position = "Posit1" });

            // Act
            var result = await _createPositionHandler.Handle(request, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }
    }
}
