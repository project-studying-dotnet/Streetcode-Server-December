using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Team;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    public class CreateTeamLinkHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateTeamLinkHandler _createTeamLinkHandler;
        private readonly TeamMemberLinkDto _teamLinkDTO = new TeamMemberLinkDto
        {
            Id = 1,
            LogoType = LogoTypeDto.YouTube,
        };

        public CreateTeamLinkHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new TeamLinkProfile())).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _createTeamLinkHandler = new CreateTeamLinkHandler(_mapper, _mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_TeamMemberLinkIsNull_ReturnsResultFail()
        {
            // Arrange
            var request = new CreateTeamLinkCommand(null);
            const string errorMsg = "Cannot convert null to team link";

            // Act
            var result = await _createTeamLinkHandler.Handle(request, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_CreatedTeamLinkIsNull_ReturnsResultFail()
        {
            // Arrange
            SetupCreateTeamLink(null);

            var request = new CreateTeamLinkCommand(_teamLinkDTO);
            const string errorMsg = "Failed to create a positions";

            // Act
            var result = await _createTeamLinkHandler.Handle(request, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesResultIsNotSuccess_ReturnsResultFail()
        {
            // Arrange
            SetupCreateTeamLink(_teamLinkDTO);
            SetupSaveChangesAsync(0);

            var request = new CreateTeamLinkCommand(_teamLinkDTO);
            const string errorMsg = "Failed to create a team";

            // Act
            var result = await _createTeamLinkHandler.Handle(request, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_CreatedTeamLinkDTOIsNotNull_ReturnsResultOK()
        {
            // Arrange
            SetupCreateTeamLink(_teamLinkDTO);
            SetupSaveChangesAsync(1);

            var request = new CreateTeamLinkCommand(_teamLinkDTO);

            // Act
            var result = await _createTeamLinkHandler.Handle(request, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_CreatedTeamLinkDTOIsNull_ReturnsResultFail()
        {
            // Arrange
            var _mockMapper = new Mock<IMapper>();
            var _handler = new CreateTeamLinkHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);

            var teamMemberLink = _mapper.Map<DAL.Entities.Team.TeamMemberLink>(_teamLinkDTO);

            SetupCreateTeamLink(_teamLinkDTO);
            SetupSaveChangesAsync(1);

            _mockMapper.Setup(m => m.Map<TeamMemberLink>(It.IsAny<TeamMemberLinkDto>())).Returns(teamMemberLink);
            _mockMapper.Setup(m => m.Map<TeamMemberLinkDto>(It.IsAny<TeamMemberLink>())).Returns((TeamMemberLinkDto)null);

            var request = new CreateTeamLinkCommand(_teamLinkDTO);
            const string errorMsg = "Failed to map created team link";

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void SetupCreateTeamLink(TeamMemberLinkDto teamLinkDTO)
        {
            var teamMemberLink = _mapper.Map<DAL.Entities.Team.TeamMemberLink>(teamLinkDTO);

            _mockRepositoryWrapper.Setup(p => p.TeamLinkRepository.Create(It.IsAny<TeamMemberLink>())).Returns(teamMemberLink);
        }

        private void SetupSaveChangesAsync(int returnVal)
        {
            _mockRepositoryWrapper.Setup(p => p.SaveChangesAsync().Result).Returns(returnVal);
        }
    }
}
