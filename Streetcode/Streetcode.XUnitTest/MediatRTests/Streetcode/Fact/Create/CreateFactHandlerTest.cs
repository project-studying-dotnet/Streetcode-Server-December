using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.Create
{
    public class CreateFactHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateFactHandler _createFactHandler;

        private readonly CreateFactDTO _createFactDto = new CreateFactDTO() { Title = "New Title", FactContent = "New Content", ImageId = 1 };

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
            const string errorMsg = "Failed to create a fact";

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
            SetupGetAll();
            SetupCreate(null);

            var command = new CreateFactCommand(_createFactDto);
            const string errorMsg = "Failed to create a fact";

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
            var fact = _mapper.Map<Entity>(_createFactDto);

            SetupGetAll();
            SetupCreate(fact);
            SetupSaveChanges(0);

            var command = new CreateFactCommand(_createFactDto);
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
            var fact = _mapper.Map<Entity>(_createFactDto);
            SetupGetAll();
            SetupCreate(fact);
            SetupSaveChanges(1);

            var command = new CreateFactCommand(_createFactDto);

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_IndexIsCorrect()
        {
            // Arrange
            SetupGetAll();

            var fact = new Entity { Title = "New Title", FactContent = "New Content", ImageId = 1, Index = 4 };
            _mockRepositoryWrapper.Setup(p => p.FactRepository.CreateAsync(It.IsAny<Entity>()).Result).Returns<Entity>(r => r);
            SetupSaveChanges(1);

            var command = new CreateFactCommand(_createFactDto);

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.Equal(4, result.Value.Index);
        }

        private void SetupGetAll()
        {
            var facts = new List<Entity>
            {
                new Entity { Id = 2, ImageId = 1, Index = 1 },
                new Entity { Id = 3, ImageId = 2, Index = 3 },
            };

            _mockRepositoryWrapper.Setup(p => p.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>,
                IIncludableQueryable<Entity, object>>>()).Result).Returns(facts);
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
