using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;
using Xunit;
using StreetcodeCoordinateEntity = Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinate;

public class UpdateCoordinateHandlerTest : AdditionalContentTestWrapper
{
    private readonly UpdateCoordinateHandler _handler;

    public UpdateCoordinateHandlerTest()
    {
        _handler = new UpdateCoordinateHandler(_repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreUpdate()
    {
        // Arrange
        var streetcodeCoordinateDto = new StreetcodeCoordinateDTO { StreetcodeId = 1 };
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { StreetcodeId = 1 };
        var request = new UpdateCoordinateCommand(streetcodeCoordinateDto);

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto))
            .Returns(streetcodeCoordinate);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.Update(streetcodeCoordinate));
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _repositoryWrapperMock.Verify(rep => rep.StreetcodeCoordinateRepository.Update(streetcodeCoordinate), Times.Once);
        _mapperMock.Verify(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto), Times.Once);
        _repositoryWrapperMock.Verify(rep => rep.SaveChangesAsync(), Times.Once());
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreNotUpdate()
    {
        // Arrange
        var streetcodeCoordinateDto = new StreetcodeCoordinateDTO { StreetcodeId = 1 };
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { StreetcodeId = 1 };
        var request = new UpdateCoordinateCommand(streetcodeCoordinateDto);
        const string errMsg = "Failed to update a streetcodeCoordinate";

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto))
            .Returns(streetcodeCoordinate);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.Update(streetcodeCoordinate));
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errMsg, result.Errors[0].Message);

        _repositoryWrapperMock.Verify(rep => rep.StreetcodeCoordinateRepository.Update(streetcodeCoordinate), Times.Once);
        _mapperMock.Verify(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto), Times.Once);
        _repositoryWrapperMock.Verify(rep => rep.SaveChangesAsync(), Times.Once());
    }
}