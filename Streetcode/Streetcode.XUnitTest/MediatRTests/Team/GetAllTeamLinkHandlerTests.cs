using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.Position.GetAll;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.GetAll;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Enums;
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
    public class GetAllTeamLinkHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllTeamLinkHandler _getAllTeamLinkHandler;

        public GetAllTeamLinkHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new TeamLinkProfile())).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _getAllTeamLinkHandler = new GetAllTeamLinkHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_TeamLinksIsNotNull_ReturnsResultOK()
        {
            // Arrange
            ArrangeTeamLinksIsNotNull();
            var request = new GetAllTeamLinkQuery();

            // Act
            var result = await _getAllTeamLinkHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async Task Handle_TeamLinksIsNull_ReturnsResultFail()
        {
            // Arrange
            ArrangeTeamLinksIsNull();
            var request = new GetAllTeamLinkQuery();
            const string errorMsg = $"Cannot find any team links";

            // Act
            var result = await _getAllTeamLinkHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangeTeamLinksIsNotNull()
        {
            List<TeamMemberLink> teamLinks = new List<TeamMemberLink>
            {
                new TeamMemberLink
                {
                    Id = 1,
                    LogoType = LogoType.YouTube,
                },
                new TeamMemberLink
                {
                    Id = 2,
                    LogoType = LogoType.Instagram,
                },
            };

            _mockRepositoryWrapper.Setup(p => p.TeamLinkRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMemberLink, bool>>>(), It.IsAny<Func<IQueryable<TeamMemberLink>,
                IIncludableQueryable<TeamMemberLink, object>>>()).Result).Returns(teamLinks);
        }

        private void ArrangeTeamLinksIsNull()
        {
            _mockRepositoryWrapper.Setup(p => p.TeamLinkRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMemberLink, bool>>>(), It.IsAny<Func<IQueryable<TeamMemberLink>,
                IIncludableQueryable<TeamMemberLink, object>>>()).Result).Returns((IEnumerable<TeamMemberLink>)null);
        }
    }
}
