using System.Linq.Expressions;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Xunit;
using StreetcodeTagIndexEntity = Streetcode.Domain.Entities.AdditionalContent.StreetcodeTagIndex;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

public class GetTagByStreetcodeIdHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetTagByStreetcodeIdHandler _handler;

    public GetTagByStreetcodeIdHandlerTest()
    {
        _handler = new GetTagByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsByStreetcodeIdAreFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetTagByStreetcodeIdQuery(streetcodeId);
        IEnumerable<StreetcodeTagIndexEntity> streetcodeTagIndexList = new[]
        {
            new StreetcodeTagIndexEntity { Index = 1, StreetcodeId = 1 },
        };
        IEnumerable<StreetcodeTagDto> streetcodeTagIndexDto = new[]
        {
            new StreetcodeTagDto { Index = 1 },
        };

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeTagIndexRepository.GetAllAsync(
                It.Is<Expression<Func<StreetcodeTagIndexEntity, bool>>>(exp => exp.Compile().Invoke(streetcodeTagIndexList.First())),
                It.IsAny<List<string>>()))
            .ReturnsAsync(streetcodeTagIndexList);

        _mapperMock.Setup(m => m.Map<IEnumerable<StreetcodeTagDto>>(streetcodeTagIndexList))
            .Returns(streetcodeTagIndexDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(streetcodeTagIndexDto, result.Value);

        _repositoryWrapperMock.Verify(
            rep => rep.StreetcodeTagIndexRepository.GetAllAsync(
            It.Is<Expression<Func<StreetcodeTagIndexEntity, bool>>>(exp => exp.Compile().Invoke(new StreetcodeTagIndexEntity { StreetcodeId = request.StreetcodeId })),
            It.IsAny<List<string>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<StreetcodeTagDto>>(streetcodeTagIndexList), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsByStreetcodeIdNotFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetTagByStreetcodeIdQuery(streetcodeId);
        string errorMsg = $"Cannot find a tag by a streetcode id: {request.StreetcodeId}";

        _repositoryWrapperMock.Setup(rep => rep.StreetcodeTagIndexRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeTagIndexEntity, bool>>>(),
                It.IsAny<List<string>>()))
            .ReturnsAsync((IEnumerable<StreetcodeTagIndexEntity>)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Reasons[0].Message);
    }
}