using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Analytics;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Analytics;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.Domain.Entities.Analytics;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Analytics.Create
{
    public class CreateStatisticRecordHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateStatisticRecordHandler _handler;

        public CreateStatisticRecordHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(StatisticRecordProfile));
            });

            _mapper = configuration.CreateMapper();
            _handler = new CreateStatisticRecordHandler(
                _mapper,
                _repositoryWrapperMock.Object,
                _loggerMock.Object
            );
        }

        [Theory]
        [InlineData(1, 1, "Address 1", 1, 1)]
        [InlineData(2, 2, "Address 2", 2, 2)]
        public async Task Handle_CreateStatisticRecord_ShouldWorkWithDifferentData(
            int qrId,
            int count,
            string address,
            int streetcodeId,
            int streetcodeCoordinateId
        )
        {
            // Arrange
            var command = new CreateStatisticRecordCommand(
                new CreateStatisticRecordDto
                {
                    QrId = qrId,
                    Count = count,
                    Address = address,
                    StreetcodeId = streetcodeId,
                    StreetcodeCoordinateId = streetcodeCoordinateId,
                }
            );

            var createStatisticRecord = command.createStatisticRecord;

            _repositoryWrapperMock
                .Setup(r => r.StatisticRecordRepository.CreateAsync(It.IsAny<StatisticRecord>()))
                .Returns(Task.FromResult(_mapper.Map<StatisticRecord>(createStatisticRecord)));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result
                .Value.Should()
                .BeEquivalentTo(
                    new StatisticRecordDto
                    {
                        QrId = qrId,
                        Count = count,
                        Address = address,
                        StreetcodeId = streetcodeId,
                        StreetcodeCoordinateId = streetcodeCoordinateId,
                    }
                );

            _repositoryWrapperMock.Verify(
                r => r.StatisticRecordRepository.CreateAsync(It.IsAny<StatisticRecord>()),
                Times.Once
            );
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task Handle_WhenCreateReturnsNull_ShouldNotCallSaveChanges()
        {
            // Arrange
            var command = new CreateStatisticRecordCommand(
                new CreateStatisticRecordDto
                {
                    QrId = 1,
                    Count = 1,
                    Address = "Test address",
                    StreetcodeId = 1,
                    StreetcodeCoordinateId = 1,
                }
            );

            _repositoryWrapperMock
                .Setup(r => r.StatisticRecordRepository.Create(It.IsAny<StatisticRecord>()))
                .Returns((StatisticRecord)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
