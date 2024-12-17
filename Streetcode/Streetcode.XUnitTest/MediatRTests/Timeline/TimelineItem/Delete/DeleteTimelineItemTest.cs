using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using TimelineEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.Delete
{
    public class DeleteTimelineItemTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly DeleteTimelineItemHandler _handler;

        public DeleteTimelineItemTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_TimelineItemIsNull_ReturnsResultFail()
        {
            // Arrange
            var command = new DeleteTimelineItemCommand(1);
            string errorMsg = $"Cannot find timeline item with Id: {command.id}";
            SetupGetFirstOrDefault(null!);

            // Act
            var result = await _handler.Handle(command, default);

            // Arrange
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesFailed_ReturnsResultFail()
        {
            // Arrange
            var command = new DeleteTimelineItemCommand(1);
            string errorMsg = $"Failed to delete timeline item";

            SetupGetFirstOrDefault(null!);

            SetupSaveChanges(0);

            // Act
            var result = await _handler.Handle(command, default);

            // Arrange
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesSucceded_ReturnsResultOk()
        {
            // Arrange
            var timelineItem = new TimelineEntity { Id = 1, Date = DateTime.Now, Title = "New Title" };
            var command = new DeleteTimelineItemCommand(1);

            SetupGetFirstOrDefault(timelineItem);

            SetupSaveChanges(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Arrange
            Assert.True(result.IsSuccess);
            Assert.Equal(Unit.Value, result.Value);
        }

        private void SetupGetFirstOrDefault(TimelineEntity timelineEntity)
        {
            _mockRepositoryWrapper.Setup(s => s.TimelineRepository.GetFirstOrDefaultAsync(
                            It.IsAny<Expression<Func<TimelineEntity, bool>>>(),
                            It.IsAny<Func<IQueryable<TimelineEntity>, IIncludableQueryable<TimelineEntity, object>>>()).Result).Returns(timelineEntity);
        }

        private void SetupSaveChanges(int returnVal)
        {
            _mockRepositoryWrapper.Setup(s => s.SaveChangesAsync().Result).Returns(returnVal);
        }
    }
}
