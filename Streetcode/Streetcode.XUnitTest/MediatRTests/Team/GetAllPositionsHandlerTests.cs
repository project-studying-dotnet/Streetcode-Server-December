using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.MediatR.Team.GetAll;
using Streetcode.BLL.MediatR.Team.Position.GetAll;
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
    public class GetAllPositionsHandlerTests
    {

        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllPositionsHandler _getAllPositionsHandler;

        public GetAllPositionsHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new PositionProfile())).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _getAllPositionsHandler = new GetAllPositionsHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_PositionIsNotNull_ReturnsResultOK()
        {
            // Arrange
            ArrangePositionsIsNotNull();
            var request = new GetAllPositionsQuery();

            // Act
            var result = await _getAllPositionsHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async Task Handle_PositionIsNull_ReturnsResultFail()
        {
            // Arrange
            ArrangePositionsIsNull();
            var request = new GetAllPositionsQuery();
            const string errorMsg = $"Cannot find any positions";

            // Act
            var result = await _getAllPositionsHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangePositionsIsNotNull()
        {
            List<Positions> positions = new List<Positions>
            {
                new Positions
                {
                    Id = 1,
                    Position = "Posit1",
                },
                new Positions
                {
                    Id = 2,
                    Position = "Posit2",
                },
            };

            _mockRepositoryWrapper.Setup(p => p.PositionRepository.GetAllAsync(
                It.IsAny<Expression<Func<Positions, bool>>>(), It.IsAny<Func<IQueryable<Positions>,
                IIncludableQueryable<Positions, object>>>()).Result).Returns(positions);
        }

        private void ArrangePositionsIsNull()
        {
            _mockRepositoryWrapper.Setup(p => p.PositionRepository.GetAllAsync(
                It.IsAny<Expression<Func<Positions, bool>>>(), It.IsAny<Func<IQueryable<Positions>,
                IIncludableQueryable<Positions, object>>>()).Result).Returns((IEnumerable<Positions>)null);
        }
    }
}
