using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Team;
using System.Linq.Expressions;
using System;
using Xunit;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.MediatR.Team.GetAll;
using AutoMapper;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class GetAllTeamHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllTeamHandler _getAllTeamHandler;

        public GetAllTeamHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _getAllTeamHandler = new GetAllTeamHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_TeamIsNotNull_ReturnsResultOK()
        {
            // Arrange
            ArrangeTeamIsNotNull();
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
            ArrangeTeamIsNull();
            var request = new GetAllTeamQuery();
            const string errorMsg = $"Cannot find any team";

            // Act
            var result = await _getAllTeamHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void ArrangeTeamIsNotNull()
        {
            List<TeamMember> teamMembers = new List<TeamMember>
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

            List<TeamMemberDTO> teamMembersDTO = new List<TeamMemberDTO>
            {
                new TeamMemberDTO
                {
                    Id = 1, FirstName = "Memb1",
                    Description = "Desc1", IsMain = true,
                    ImageId = 1,
                },
                new TeamMemberDTO
                {
                    Id = 2, FirstName = "Memb2",
                    Description = "Desc2", IsMain = false,
                    ImageId = 2,
                },
            };

            _mockRepositoryWrapper.Setup(p => p.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<Func<IQueryable<TeamMember>,
                IIncludableQueryable<TeamMember, object>>>()).Result).Returns(teamMembers);

            _mockMapper.Setup(m => m.Map<IEnumerable<TeamMemberDTO>>(It.IsAny<IEnumerable<TeamMember>>())).Returns(
                teamMembersDTO);
        }

        private void ArrangeTeamIsNull()
        {
            _mockRepositoryWrapper.Setup(p => p.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<Func<IQueryable<TeamMember>,
                IIncludableQueryable<TeamMember, object>>>()).Result).Returns((IEnumerable<TeamMember>)null);
        }
    }
}
