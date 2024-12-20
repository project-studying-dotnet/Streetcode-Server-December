using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.BLL.Specifications.Streetcode.Streetcode;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text.Create
{
    public class CreateTextHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateTextHandler _handler;

        public CreateTextHandlerTests()
        {
            _mockLogger = new Mock<ILoggerService>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(TextProfile));
            });
            _mapper = configuration.CreateMapper();
            _handler = new CreateTextHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenStreetcodeDoesNotExist_ThrowsEntityNotFoundException()
        {
            ConfigureStreetcodeRepository(null);
            var textDTO = CreateTextDTO();

            var command = new CreateTextCommand(textDTO);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_FailedSaveChanges_ShouldThrowException()
        {
            var textDTO = CreateTextDTO();
            var streetcodeContent = CreateStreetcodeContent();

            ConfigureTextRepository(textDTO);
            ConfigureStreetcodeRepository(streetcodeContent);
            ConfigureSaveChanges(0);

            var command = new CreateTextCommand(textDTO);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), "Failed to save changes to the database."), Times.Once);
        }

        [Fact]
        public async Task Handle_SuccessfulExecution_ShouldReturnOkResult()
        {
            // Arrange
            var textDTO = CreateTextDTO();
            var streetcodeContent = CreateStreetcodeContent();

            ConfigureTextRepository(textDTO);
            ConfigureStreetcodeRepository(streetcodeContent);
            ConfigureSaveChanges(1);

            var command = new CreateTextCommand(textDTO);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be(textDTO.Title);

            _mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        private void ConfigureStreetcodeRepository(StreetcodeContent? streetcodeContent)
        {
            _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
                .GetFirstOrDefaultBySpecAsync(It.IsAny<GetByStreetcodeIdSpecification>()))
                .ReturnsAsync(streetcodeContent);
        }

        private void ConfigureTextRepository(TextCreateDTO? text)
        {
            _mockRepositoryWrapper.Setup(repo => repo.TextRepository.CreateAsync(It.IsAny<TextEntity>()))
                .ReturnsAsync(_mapper.Map<TextEntity>(text));
        }

        private void ConfigureSaveChanges(int isSucess)
        {
            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(isSucess);
        }

        private TextCreateDTO CreateTextDTO()
        {
            var textCreateDto = new TextCreateDTO
            {
                StreetcodeId = 1,
                Title = "Title to create",
                TextContent = "Content to create"
            };
            return textCreateDto;
        }

        private StreetcodeContent CreateStreetcodeContent()
        {
            var streetcodeContent = new StreetcodeContent
            {
                Id = 1,
                Title = "Title",
            };
            return streetcodeContent;
        }
    }
}
