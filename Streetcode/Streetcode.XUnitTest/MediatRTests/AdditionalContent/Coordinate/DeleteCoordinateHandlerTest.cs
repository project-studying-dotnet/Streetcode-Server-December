using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Xunit;
using StreetcodeCoordinateEntity = Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinate;

public class DeleteCoordinateHandlerTest : AdditionalContentTestWrapper
{
    private readonly DeleteCoordinateHandler _handler;

    public DeleteCoordinateHandlerTest()
    {
        _handler = new DeleteCoordinateHandler(_repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreDelete()
    {
        // Arrange
        var streetcodeCoordinateId = 1;
        var request = new DeleteCoordinateCommand(streetcodeCoordinateId);
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { Id = 1 };

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<StreetcodeCoordinateEntity, bool>>>(exp => exp.Compile().Invoke(streetcodeCoordinate)),
                    It.IsAny<Func<IQueryable<StreetcodeCoordinateEntity>, IIncludableQueryable<StreetcodeCoordinateEntity, object>>>()))
            .ReturnsAsync(streetcodeCoordinate);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.Delete(streetcodeCoordinate));
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var res = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreNotDelete()
    {
        // Arrange
        var streetcodeCoordinateId = 1;
        var request = new DeleteCoordinateCommand(streetcodeCoordinateId);
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { Id = 1 };

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<StreetcodeCoordinateEntity, bool>>>(exp => exp.Compile().Invoke(streetcodeCoordinate)),
                    It.IsAny<Func<IQueryable<StreetcodeCoordinateEntity>, IIncludableQueryable<StreetcodeCoordinateEntity, object>>>()))
            .ReturnsAsync(streetcodeCoordinate);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.Delete(streetcodeCoordinate));
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var res = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(res.IsFailed);
    }
}