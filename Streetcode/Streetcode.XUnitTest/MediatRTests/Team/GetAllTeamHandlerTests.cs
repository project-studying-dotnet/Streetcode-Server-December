using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using System.Linq.Expressions;
using Xunit;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.MediatR.Team.GetAll;
using AutoMapper;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.BLL.Mapping.Team;
using Streetcode.Domain.Entities.Team;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class GetAllTeamHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly IMapper _mapper;
        private readonly GetAllTeamHandler _getAllTeamHandler;

        public GetAllTeamHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new TeamProfile())).CreateMapper();
            _mockLogger = new Mock<ILoggerService>();
            _getAllTeamHandler = new GetAllTeamHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
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
            var request = new GetAllTeamQuery();

            // Act
            var result = await _getAllTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async Task Handle_TeamIsNull_ReturnsResultFail()
        {
            // Arrange
            ArrangeTeams(null);
            var request = new GetAllTeamQuery();
            const string errorMsg = $"Cannot find any team";

            // Act
            var result = await _getAllTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangeTeams(List<TeamMember> teamMembers)
        {
            _mockRepositoryWrapper.Setup(p => p.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<Func<IQueryable<TeamMember>,
                IIncludableQueryable<TeamMember, object>>>()).Result).Returns(teamMembers);
        }
    }
}
