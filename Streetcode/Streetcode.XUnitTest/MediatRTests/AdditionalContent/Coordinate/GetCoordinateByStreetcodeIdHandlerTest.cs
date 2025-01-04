using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode;
using Xunit;
using StreetcodeCoordinateEntity = Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinate;

public class GetCoordinateByStreetcodeIdHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetCoordinatesByStreetcodeIdHandler _handler;

    public GetCoordinateByStreetcodeIdHandlerTest()
    {
        _handler = new GetCoordinatesByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateAreFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetCoordinatesByStreetcodeIdQuery(streetcodeId);

        var streetcodeContent = new StreetcodeContent { Id = 1, };
        IEnumerable<StreetcodeCoordinateEntity> streetcodeCoordinateList = new[]
        {
            new StreetcodeCoordinateEntity { Id = 1, StreetcodeId = 1 },
        };
        IEnumerable<StreetcodeCoordinateDto> streetcodeCoordinateDto = new[]
        {
            new StreetcodeCoordinateDto { Id = 1, StreetcodeId = 1 },
        };

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcodeContent)),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcodeContent);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.GetAllAsync(
                It.Is<Expression<Func<StreetcodeCoordinateEntity, bool>>>(exp =>
                    exp.Compile().Invoke(streetcodeCoordinateList.First())),
                It.IsAny<Func<IQueryable<StreetcodeCoordinateEntity>,
                    IIncludableQueryable<StreetcodeCoordinateEntity, object>>>()))
            .ReturnsAsync(streetcodeCoordinateList);

        _mapperMock.Setup(rep => rep.Map<IEnumerable<StreetcodeCoordinateDto>>(streetcodeCoordinateList))
            .Returns(streetcodeCoordinateDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(streetcodeCoordinateDto, result.Value);

        _repositoryWrapperMock.Verify(
            rep => rep.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<StreetcodeContent, bool>>>(
                    exp => exp.Compile().Invoke(new StreetcodeContent { Id = streetcodeContent.Id })),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()), Times.Once);

        _repositoryWrapperMock.Verify(
            rep => rep.StreetcodeCoordinateRepository.GetAllAsync(
                It.Is<Expression<Func<StreetcodeCoordinateEntity, bool>>>(
                    exp => exp.Compile().Invoke(new StreetcodeCoordinateEntity { StreetcodeId = request.StreetcodeId })),
                It.IsAny<Func<IQueryable<StreetcodeCoordinateEntity>, IIncludableQueryable<StreetcodeCoordinateEntity, object>>>()), Times.Once);

        _mapperMock.Verify(m => m.Map<IEnumerable<StreetcodeCoordinateDto>>(streetcodeCoordinateList), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCoordinateNotFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetCoordinatesByStreetcodeIdQuery(streetcodeId);
        string errorMsg = $"Cannot find a coordinate by a streetcode id: {request.StreetcodeId}";
        var streetcodeContent = new StreetcodeContent { Id = 1, };

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcodeContent)),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcodeContent);

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeCoordinateRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinateEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeCoordinateEntity>,
                    IIncludableQueryable<StreetcodeCoordinateEntity, object>>>()))
            .ReturnsAsync((IEnumerable<StreetcodeCoordinateEntity>)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors[0].Message);

        _repositoryWrapperMock.Verify(
            rep => rep.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<StreetcodeContent, bool>>>(
                    exp => exp.Compile().Invoke(new StreetcodeContent { Id = streetcodeContent.Id })),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()), Times.Once);

        _repositoryWrapperMock.Verify(
            rep => rep.StreetcodeCoordinateRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinateEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeCoordinateEntity>, IIncludableQueryable<StreetcodeCoordinateEntity, object>>>()), Times.Once);
    }
}