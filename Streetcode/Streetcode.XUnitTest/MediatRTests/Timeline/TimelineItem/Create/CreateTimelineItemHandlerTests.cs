using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Entities.Timeline;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItems.Create
{
    /// <summary>
    /// Unit tests for <see cref="CreateTimelineItemHandler"/>.
    /// </summary>
    public class CreateTimelineItemHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateTimelineItemHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTimelineItemHandlerTests"/> class.
        /// Sets up required dependencies and configurations.
        /// </summary>
        public CreateTimelineItemHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(TimelineItemProfile));
            });

            _mapper = configuration.CreateMapper();
            _handler = new CreateTimelineItemHandler(_mapper, _repositoryWrapperMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Tests if the handler returns a failed result when the Streetcode does not exist.
        /// </summary>
        /// <returns>A task representing the asynchronous test.</returns>
        [Fact]
        public async Task Handle_StreetcodeDoesNotExist_ReturnsFailResult()
        {
            // Arrange
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto { StreetcodeId = 1 });
            SetupStreetcodeRepositoryToReturnNull();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(error => error.Message == "Cannot find any streetcode");
        }

        /// <summary>
        /// Tests if the handler successfully creates a timeline without historical contexts when none are provided.
        /// </summary>
        /// <returns>A task representing the asynchronous test.</returns>
        [Fact]
        public async Task Handle_HistoricalContextsEmpty_CreatesTimelineWithoutContexts()
        {
            // Arrange
            var command = CreateCommandWithoutHistoricalContexts();
            var timelineEntity = command.timelineItemCreateDto;
            SetupStreetcodeRepositoryToReturnValidEntity();

            _repositoryWrapperMock.Setup(r => r.TimelineRepository.CreateAsync(It.IsAny<TimelineItem>()))
                                  .ReturnsAsync(_mapper.Map<TimelineItem>(timelineEntity));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be(timelineEntity.Title);
        }

        /// <summary>
        /// Tests if the handler successfully creates a timeline with provided historical contexts.
        /// </summary>
        /// <returns>A task representing the asynchronous test.</returns>
        [Fact]
        public async Task Handle_HistoricalContextsProvided_CreatesTimelineWithContexts()
        {
            // Arrange
            var command = CreateCommandWithHistoricalContexts();
            var timelineEntity = command.timelineItemCreateDto;
            SetupStreetcodeRepositoryToReturnValidEntity();
            SetupHistoricalContextsRepository();

            _repositoryWrapperMock.Setup(r => r.TimelineRepository.CreateAsync(It.IsAny<TimelineItem>()))
                                  .ReturnsAsync(_mapper.Map<TimelineItem>(timelineEntity));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be(timelineEntity.Title);
            result.Value.HistoricalContexts.Should().HaveCount(2);
        }

        /// <summary>
        /// Configures the StreetcodeRepository mock to return null for any request.
        /// </summary>
        private void SetupStreetcodeRepositoryToReturnNull()
        {
            _repositoryWrapperMock.Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync((StreetcodeContent?)null);
        }

        /// <summary>
        /// Configures the StreetcodeRepository mock to return a valid entity for any request.
        /// </summary>
        private void SetupStreetcodeRepositoryToReturnValidEntity()
        {
            _repositoryWrapperMock.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(new StreetcodeContent { Id = 1 });
        }

        /// <summary>
        /// Configures the HistoricalContextRepository mock to return a filtered list of historical contexts.
        /// </summary>
        private void SetupHistoricalContextsRepository()
        {
            var historicalContextEntities = new List<HistoricalContext>
            {
                new() { Id = 1, Title = "Context 1" },
                new() { Id = 2, Title = "Context 2" },
                new() { Id = 3, Title = "Context 3" }
            };

            _repositoryWrapperMock.Setup(r => r.HistoricalContextRepository.GetAllAsync(
                    It.IsAny<Expression<Func<HistoricalContext, bool>>>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync((Expression<Func<HistoricalContext, bool>> predicate, Func<IQueryable<HistoricalContext>, IIncludableQueryable<HistoricalContext, object>> include) =>
                    historicalContextEntities.AsQueryable().Where(predicate));
        }

        /// <summary>
        /// Creates a test command without historical contexts.
        /// </summary>
        /// <returns>A <see cref="CreateTimelineItemCommand"/> instance.</returns>
        private CreateTimelineItemCommand CreateCommandWithoutHistoricalContexts()
        {
            return new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Date = DateTime.UtcNow,
                Title = "Event 1",
                Description = "Description 1",
                HistoricalContexts = new List<HistoricalContextDto>(),
                StreetcodeId = 1
            });
        }

        /// <summary>
        /// Creates a test command with historical contexts.
        /// </summary>
        /// <returns>A <see cref="CreateTimelineItemCommand"/> instance.</returns>
        private CreateTimelineItemCommand CreateCommandWithHistoricalContexts()
        {
            return new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                StreetcodeId = 1,
                Title = "Test title",
                HistoricalContexts = new List<HistoricalContextDto>
                {
                    new() { Id = 1, Title = "Context 1" },
                    new() { Id = 2, Title = "Context 2" }
                },
            });
        }
    }
}
