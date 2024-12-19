using System.Reflection;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.MediatR.Analytics;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Analytics;
using Xunit;

namespace Streetcode.XUnitTest.ControllerTests.Analytics
{
    public class StatisticRecordControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly StatisticRecordController _controller;

        public StatisticRecordControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new StatisticRecordController();

            var mediatorField = typeof(BaseApiController).GetField(
                "_mediator",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            mediatorField.SetValue(_controller, _mediatorMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WhenCommandIsSuccessful()
        {
            var createDto = new CreateStatisticRecordDTO
            {
                QrId = 1,
                Count = 2,
                Address = "Test address",
                StreetcodeId = 1,
                StreetcodeCoordinateId = 1,
            };

            var responseDto = new StatisticRecordDTO
            {
                Id = 1,
                QrId = createDto.QrId,
                Count = createDto.Count,
                Address = createDto.Address,
                StreetcodeId = createDto.StreetcodeId,
                StreetcodeCoordinateId = createDto.StreetcodeCoordinateId,
            };

            var command = new CreateStatisticRecordCommand(createDto);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateStatisticRecordCommand>(), default))
                .ReturnsAsync(Result.Ok(responseDto));

            var result = await _controller.Create(createDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<CreateStatisticRecordCommand>(), default),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCommandFails()
        {
            var createDto = new CreateStatisticRecordDTO
            {
                QrId = 1,
                Count = 2,
                Address = "Test address",
                StreetcodeId = 1,
                StreetcodeCoordinateId = 1,
            };

            var command = new CreateStatisticRecordCommand(createDto);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateStatisticRecordCommand>(), default))
                .ReturnsAsync(Result.Fail("Failed to create record"));

            var result = await _controller.Create(createDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<CreateStatisticRecordCommand>(), default),
                Times.Once
            );
        }
    }
}
