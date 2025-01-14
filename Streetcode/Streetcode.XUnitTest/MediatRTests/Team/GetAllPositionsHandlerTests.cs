using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.Position.GetAll;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Team;
using System.Linq.Expressions;
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
            var positions = new List<Positions>
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

            ArrangePositions(positions);
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
            ArrangePositions(null);
            var request = new GetAllPositionsQuery();
            const string errorMsg = $"Cannot find any positions";

            // Act
            var result = await _getAllPositionsHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangePositions(List<Positions> positions)
        {
            _mockRepositoryWrapper.Setup(p => p.PositionRepository.GetAllAsync(
                It.IsAny<Expression<Func<Positions, bool>>>(), It.IsAny<List<string>>()).Result).Returns(positions);
        }
    }
}
