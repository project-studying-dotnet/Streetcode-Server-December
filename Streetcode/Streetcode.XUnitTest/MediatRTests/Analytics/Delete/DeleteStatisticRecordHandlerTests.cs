using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Analytics.Delete;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Analytics.Delete
{
    public class DeleteStatisticRecordHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly DeleteStatisticRecordHandler _handler;

        public DeleteStatisticRecordHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new DeleteStatisticRecordHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_SaveChangesFailed_ReturnsResultFail()
        {
            var statisticRecordItem = new StatisticRecord
            {
                Id = 1,
                QrId = 1,
                Count = 5,
                Address = "Test address",
                StreetcodeId = 1,
                StreetcodeCoordinateId = 1
            };
            var command = new DeleteStatisticRecordCommand(1);

            SetupGetFirstOrDefault(statisticRecordItem);
            SetupSaveChanges(0);

            var result = await _handler.Handle(command, default);

            Assert.False(result.IsSuccess);
            Assert.Contains("Failed to delete a StatisticRecord", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_RecordNotFound_ReturnsResultFail()
        {
            var command = new DeleteStatisticRecordCommand(1);

            SetupGetFirstOrDefault(null);

            var result = await _handler.Handle(command, default);

            Assert.False(result.IsSuccess);
            Assert.Contains($"Cannot find a StatisticRecord with corresponding id: {command.id}", result.Errors.First().Message);
        }

        private void SetupGetFirstOrDefault(StatisticRecord statisticRecord)
        {
            _mockRepositoryWrapper.Setup(s => s.StatisticRecordRepository.GetFirstOrDefaultAsync(
                            It.IsAny<Expression<Func<StatisticRecord, bool>>>(),
                            It.IsAny<Func<IQueryable<StatisticRecord>, IIncludableQueryable<StatisticRecord, object>>>()
            )).ReturnsAsync(statisticRecord);
        }

        private void SetupSaveChanges(int returnVal)
        {
            _mockRepositoryWrapper.Setup(s => s.SaveChangesAsync()).ReturnsAsync(returnVal);
        }
    }
}