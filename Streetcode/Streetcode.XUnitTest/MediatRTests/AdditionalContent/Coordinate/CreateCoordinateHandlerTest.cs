using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Xunit;
using StreetcodeCoordinateEntity = Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinate;

public class CreateCoordinateHandlerTest : AdditionalContentTestWrapper
{
    private readonly CreateCoordinateHandler _handler;

    public CreateCoordinateHandlerTest()
    {
        _handler = new CreateCoordinateHandler(_repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreCreate()
    {
        // Arrange
        var streetcodeCoordinateDto = new StreetcodeCoordinateDto { StreetcodeId = 1 };
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { StreetcodeId = 1 };
        var request = new CreateCoordinateCommand(streetcodeCoordinateDto);

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto))
            .Returns(streetcodeCoordinate);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.CreateAsync(It.IsAny<StreetcodeCoordinateEntity>()))
            .Returns(Task.FromResult(streetcodeCoordinate));
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _repositoryWrapperMock.Verify(rep => rep.StreetcodeCoordinateRepository.CreateAsync(streetcodeCoordinate), Times.Once);
        _mapperMock.Verify(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto), Times.Once);
        _repositoryWrapperMock.Verify(rep => rep.SaveChangesAsync(), Times.Once());
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreNotCreate()
    {
        // Arrange
        var streetcodeCoordinateDto = new StreetcodeCoordinateDto { StreetcodeId = 1 };
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { StreetcodeId = 1 };
        var request = new CreateCoordinateCommand(streetcodeCoordinateDto);
        const string errMsg = "Failed to create a streetcodeCoordinate";

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto))
            .Returns(streetcodeCoordinate);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.CreateAsync(streetcodeCoordinate))
            .Returns(Task.FromResult((StreetcodeCoordinateEntity)null!));
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errMsg, result.Errors[0].Message);

        _repositoryWrapperMock.Verify(rep => rep.StreetcodeCoordinateRepository.CreateAsync(streetcodeCoordinate), Times.Once);
        _mapperMock.Verify(m => m.Map<StreetcodeCoordinateEntity>(streetcodeCoordinateDto), Times.Once);
        _repositoryWrapperMock.Verify(rep => rep.SaveChangesAsync(), Times.Once());
    }
}