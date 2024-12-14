using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact
{
    public class CreateFactHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateFactHandler _createFactHandler;

        private readonly FactDto _factDto = new FactDto() { Id = 1, Title = "New Title", FactContent = "New Content", ImageId = 1 };

        public CreateFactHandlerTest()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new FactProfile())).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _createFactHandler = new CreateFactHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_FactIsNull_ReturnsResultFail()
        {
            // Arrange
            var command = new CreateFactCommand(null);
            const string errorMsg = "Cannot create new fact!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_CreatedFactIsNull_ReturnsResultFail()
        {
            // Arrange
            SetupCreate(null);

            var command = new CreateFactCommand(_factDto);
            const string errorMsg = "Cannot create fact!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesResultIsNotSuccess_ReturnsResultFail()
        {
            // Arrange
            var fact = _mapper.Map<Entity>(_factDto);

            SetupCreate(fact);
            SetupSaveChanges(0);

            var command = new CreateFactCommand(_factDto);
            const string errorMsg = "Cannot save changes in the database!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_FactCreatedSuccessfully_ReturnsResultOK()
        {
            // Arrange
            var fact = _mapper.Map<Entity>(_factDto);
            SetupCreate(fact);
            SetupSaveChanges(1);

            var command = new CreateFactCommand(_factDto);

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        private void SetupCreate(Entity fact)
        {
            _mockRepositoryWrapper.Setup(p => p.FactRepository.CreateAsync(It.IsAny<Entity>()).Result).Returns(fact);
        }

        private void SetupSaveChanges(int returnValue)
        {
            _mockRepositoryWrapper.Setup(p => p.SaveChangesAsync().Result).Returns(returnValue);
        }
    }
}
