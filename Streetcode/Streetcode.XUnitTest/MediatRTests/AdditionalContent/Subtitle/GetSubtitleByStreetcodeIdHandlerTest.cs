using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetById;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using SubtitleEntity = Streetcode.DAL.Entities.AdditionalContent.Subtitle;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Subtitle;

public class GetSubtitleByStreetcodeIdHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetSubtitlesByStreetcodeIdHandler _handler;

    public GetSubtitleByStreetcodeIdHandlerTest()
    {
        _handler = new GetSubtitlesByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task Handler_ReturnsSubtitleByStreetcodeIdDto_WhenSubtitleExist()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetSubtitlesByStreetcodeIdQuery(streetcodeId);
        var subtitle = new SubtitleEntity
        {
            Id = 1,
            SubtitleText = "Test subtitle 1",
            StreetcodeId = 1,
        };
        var subtitleDto = new SubtitleDTO
        {
            Id = 1,
            SubtitleText = "Test subtitle 1",
            StreetcodeId = 1,
        };

        _repositoryWrapperMock.Setup(rep => rep.SubtitleRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<SubtitleEntity, bool>>>(exp => exp.Compile().Invoke(subtitle)),
                    It.IsAny<Func<IQueryable<SubtitleEntity>, IIncludableQueryable<SubtitleEntity, object>>>()))
            .ReturnsAsync(subtitle);

        _mapperMock.Setup(m => m.Map<SubtitleDTO>(subtitle))
            .Returns(subtitleDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(subtitleDto, result.Value);

        _repositoryWrapperMock.Verify(
            rep => rep.SubtitleRepository.GetFirstOrDefaultAsync(
            It.Is<Expression<Func<SubtitleEntity, bool>>>(
                exp => exp.Compile().Invoke(new SubtitleEntity { StreetcodeId = request.StreetcodeId })),
            It.IsAny<Func<IQueryable<SubtitleEntity>, IIncludableQueryable<SubtitleEntity, object>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<SubtitleDTO>(subtitle), Times.Once);
    }

    // [Fact]
    // public async Task Handler_ReturnsSubtitleByStreetcodeIdDto_WhenSubtitleNotFound()
    // {
    //     // Arrange
    //     var streetcodeId = 1;
    //     var request = new GetSubtitlesByStreetcodeIdQuery(streetcodeId);
    //     string errorMsg = $"Cannot find a subtitle with corresponding id: {request.StreetcodeId}";
    //
    //
    //     _repositoryWrapperMock.Setup(rep => rep.SubtitleRepository
    //             .GetFirstOrDefaultAsync(
    //                 It.IsAny<Expression<Func<SubtitleEntity, bool>>>(),
    //                 It.IsAny<Func<IQueryable<SubtitleEntity>, IIncludableQueryable<SubtitleEntity, object>>>()))
    //         .ReturnsAsync((SubtitleEntity)null!);
    //
    //     // Act
    //     var result = await _handler.Handle(request, CancellationToken.None);
    //
    //     // Assert
    //     Assert.Equal(errorMsg, result.Errors[0].Message);
    // }
}