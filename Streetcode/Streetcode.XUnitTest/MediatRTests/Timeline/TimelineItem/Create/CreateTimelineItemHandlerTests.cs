
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItems.Create
{
    /// <summary>
    /// Test class for CreateTimelineItemHandler.
    /// </summary>
    public class CreateTimelineItemHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateTimelineItemHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTimelineItemHandlerTests"/> class.
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
        /// Handle situation when streetcode does not exist, should return fail result.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_StreetcodeDoesNotExist_ReturnsFailResult()
        {
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto { StreetcodeId = 1 });
            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync((StreetcodeContent?)null);
            var result = await _handler.Handle(command, CancellationToken.None);
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().Message.Should().Be("Streetcode does not exist.");
            _loggerMock.Verify(l => l.LogError(command, "Streetcode does not exist."), Times.Once);
        }

        /// <summary>
        /// Handle situation when historical context is empty, should create timeline without it.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_HistoricalContextsEmpty_CreatesTimelineWithoutContexts()
        {
            // Arrange
            var newtimeLine = CreateTimeLine(); // Function to create a sample TImelineItemCreateDto
            var streetcode = new StreetcodeContent { Id = 1 }; // Sample Streetcode entity
            var timelineEntity = _mapper.Map<TimelineItem>(newtimeLine);
            var command = new CreateTimelineItemCommand(newtimeLine);

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _repositoryWrapperMock.Setup(r => r.TimelineRepository.CreateAsync(It.IsAny<TimelineItem>()))
                .ReturnsAsync(timelineEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be(newtimeLine.Title);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Handle situation when historical context is not empty, should create timeline with it successfull.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_HistoricalContextsProvided_CreatesTimelineWithContexts()
        {
            // Arrange
        }

        private TimelineItemCreateDto CreateTimeLine()
        {
            return new TimelineItemCreateDto
            {
                Date = DateTime.UtcNow,
                Title = "Event 1",
                Description = "Description 1",
                HistoricalContexts = new List<HistoricalContextDTO>(),
                StreetcodeId = 1,
            };
        }
    }
}