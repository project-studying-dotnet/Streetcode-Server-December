using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Update;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItems.Update
{
    /// <summary>
    /// Unit tests for the UpdateTimelineItemHandler class.
    /// </summary>
    public class UpdateTimelineHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly UpdateTimelineItemHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTimelineHandlerTests"/> class.
        /// </summary>
        public UpdateTimelineHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TimelineItemProfile>());
            _mapper = configuration.CreateMapper();

            _handler = new UpdateTimelineItemHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
        }

        /// <summary>
        /// Tests if the handler successfully updates a timeline when the timeline exists.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldUpdateTimeline_WhenTimelineExists()
        {
            // Arrange
            var existingTimelineItem = CreateExistingTimelineItem();
            var updatedTimelineItemDto = CreateUpdatedTimelineItemDto();

            SetupMocksForExistingTimeline(existingTimelineItem);
            SetupMocksForHistoricalContexts();
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new UpdateTimelineItemCommand(updatedTimelineItemDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be(updatedTimelineItemDto.Title);
            result.Value.Description.Should().Be(updatedTimelineItemDto.Description);
            result.Value.HistoricalContexts.Should().HaveCount(2);
        }

        /// <summary>
        /// Tests if the handler returns failure when the timeline does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenTimelineDoesNotExist()
        {
            // Arrange
            _repositoryWrapperMock.Setup(r => r.TimelineRepository
               .GetFirstOrDefaultAsync(
                   It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                   It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>()))
               .ReturnsAsync((TimelineItem)null!);

            var updatedTimelineItemDto = CreateUpdatedTimelineItemDto();
            var command = new UpdateTimelineItemCommand(updatedTimelineItemDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().Message.Should().Be("Cannot find any timeline");
            VerifySaveChangesAsync(Times.Never());
            VerifyRepositoryUpdate(Times.Never());
        }

        /// <summary>
        /// Tests if the handler returns failure when saving changes fails.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
        {
            // Arrange
            var existingTimelineItem = CreateExistingTimelineItem();
            var updatedTimelineItemDto = CreateUpdatedTimelineItemDto();

            SetupMocksForExistingTimeline(existingTimelineItem);
            SetupMocksForHistoricalContexts();
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var command = new UpdateTimelineItemCommand(updatedTimelineItemDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().Message.Should().Be("Failed to update a timeline");
        }

        /// <summary>
        /// Tests if the handler logs an error when the timeline does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldLogError_WhenTimelineDoesNotExist()
        {
            // Arrange
            SetupMocksForExistingTimeline(null!);

            var updatedTimelineItemDto = CreateUpdatedTimelineItemDto();
            var command = new UpdateTimelineItemCommand(updatedTimelineItemDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.LogError(
                It.IsAny<object>(),
                It.Is<string>(msg => msg.Contains("Cannot find any timeline"))),
                Times.Once());
        }

        /// <summary>
        /// Tests if the handler updates the timeline when historical contexts are null or empty.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldUpdateTimeline_WhenHistoricalContextsAreNullOrEmpty()
        {
            // Arrange
            var existingTimelineItem = CreateExistingTimelineItem();
            var updatedTimelineItemDto = CreateUpdatedTimelineItemDtoWithNullHistoricalContexts();

            SetupMocksForExistingTimeline(existingTimelineItem);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new UpdateTimelineItemCommand(updatedTimelineItemDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be(updatedTimelineItemDto.Title);
            result.Value.Description.Should().Be(updatedTimelineItemDto.Description);
            result.Value.HistoricalContexts.Should().BeNullOrEmpty();
        }

        private void SetupMocksForExistingTimeline(TimelineItem existingTimelineItem)
        {
            _repositoryWrapperMock.Setup(r => r.TimelineRepository
               .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TimelineItem, bool>>>(),
               It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>()))
               .ReturnsAsync(existingTimelineItem);
        }

        private void SetupMocksForHistoricalContexts()
        {
            _repositoryWrapperMock.Setup(r => r.HistoricalContextRepository
                .GetAllAsync(It.IsAny<Expression<Func<HistoricalContext, bool>>>(), null))
                .ReturnsAsync(new List<HistoricalContext>
                {
                    new HistoricalContext { Id = 1, Title = "Context 1" },
                    new HistoricalContext { Id = 5, Title = "Context 5" }
                });
        }

        private void VerifyRepositoryUpdate(Times times)
        {
            _repositoryWrapperMock.Verify(r => r.TimelineRepository.Update(It.IsAny<TimelineItem>()), times);
        }

        private void VerifySaveChangesAsync(Times times)
        {
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), times);
        }

        private TimelineItem CreateExistingTimelineItem()
        {
            return new TimelineItem
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                Date = new DateTime(1999, 8, 7),
                DateViewPattern = DateViewPattern.SeasonYear,
                HistoricalContextTimelines = new List<HistoricalContextTimeline>()
            };
        }

        private TimelineItemUpdateDto CreateUpdatedTimelineItemDto()
        {
            return new TimelineItemUpdateDto
            {
                Id = 1,
                Title = "Updated Title",
                Description = "Updated Description",
                Date = new DateTime(2004, 3, 13),
                DateViewPattern = DateViewPattern.DateMonthYear,
                HistoricalContexts = new List<HistoricalContextDto>
                {
                    new HistoricalContextDto { Id = 1, Title = "Context 1" },
                    new HistoricalContextDto { Id = 5, Title = "Context 5" }
                }
            };
        }

        private TimelineItemUpdateDto CreateUpdatedTimelineItemDtoWithNullHistoricalContexts()
        {
            return new TimelineItemUpdateDto
            {
                Id = 1,
                Title = "Updated Title With Null Contexts",
                Description = "Updated Description With Null Contexts",
                Date = new DateTime(2004, 3, 13),
                DateViewPattern = DateViewPattern.DateMonthYear,
                HistoricalContexts = null
            };
        }
    }
}
