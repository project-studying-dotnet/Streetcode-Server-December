using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.GetById;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class GetByIdTeamHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly IMapper _mapper;
        private readonly GetByIdTeamHandler _getByIdTeamHandler;

        public GetByIdTeamHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new TeamProfile())).CreateMapper();
            _mockLogger = new Mock<ILoggerService>();
            _getByIdTeamHandler = new GetByIdTeamHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_TeamIsNotNull_ReturnsResultOK()
        {
            // Arrange
            var teamMember = new TeamMember
            {
                Id = 1,
                FirstName = "Memb1",
                Description = "Desc1",
                IsMain = true,
                ImageId = 1,
            };
            var id = 1;

            ArrangeTeams(teamMember);
            var request = new GetByIdTeamQuery(id);

            // Act
            var result = await _getByIdTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_TeamIsNull_ReturnsResultFail()
        {
            // Arrange
            ArrangeTeams(null);

            var id = 1;
            var request = new GetByIdTeamQuery(id);

            string errorMsg = $"Cannot find any team with corresponding id: {request.Id}";

            // Act
            var result = await _getByIdTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangeTeams(TeamMember teamMember)
        {
            _mockRepositoryWrapper.Setup(p => p.TeamRepository.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<Func<IQueryable<TeamMember>,
                IIncludableQueryable<TeamMember, object>>>()).Result).Returns(teamMember);
        }
    }
}
