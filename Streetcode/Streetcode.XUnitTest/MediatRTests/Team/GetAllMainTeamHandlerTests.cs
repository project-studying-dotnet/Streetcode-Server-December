using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.GetAll;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Team;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class GetAllMainTeamHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly IMapper _mapper;
        private readonly GetAllMainTeamHandler _getAllMainTeamHandler;

        public GetAllMainTeamHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new TeamProfile())).CreateMapper();
            _mockLogger = new Mock<ILoggerService>();
            _getAllMainTeamHandler = new GetAllMainTeamHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_TeamIsNotNull_ReturnsResultOK()
        {
            // Arrange
            var teamMembers = new List<TeamMember>
            {
                new TeamMember
                {
                    Id = 1, FirstName = "Memb1",
                    Description = "Desc1", IsMain = true,
                    ImageId = 1,
                },
                new TeamMember
                {
                    Id = 2, FirstName = "Memb2",
                    Description = "Desc2", IsMain = false,
                    ImageId = 2,
                },
            };
            ArrangeTeams(teamMembers);
            var request = new GetAllMainTeamQuery();

            // Act
            var result = await _getAllMainTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async Task Handle_TeamIsNull_ReturnsResultFail()
        {
            // Arrange
            ArrangeTeams(null);
            var request = new GetAllMainTeamQuery();
            const string errorMsg = $"Cannot find any team";

            // Act
            var result = await _getAllMainTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangeTeams(List<TeamMember> teamMembers)
        {
            _mockRepositoryWrapper.Setup(p => p.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<List<string>>()).Result).Returns(teamMembers);
        }
    }
}
