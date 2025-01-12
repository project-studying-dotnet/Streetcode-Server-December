using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetAll;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using SubtitleEntity = Streetcode.Domain.Entities.AdditionalContent.Subtitle;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Subtitle;

public class GetAllSubtitleHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetAllSubtitlesHandler _handler;

    public GetAllSubtitleHandlerTest()
    {
        _handler = new GetAllSubtitlesHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handler_ReturnsSubtitleDto_WhenSubtitleExist()
    {
        // Arrange
        var subtitle = new List<SubtitleEntity>
        {
            new SubtitleEntity
            {
                Id = 1, SubtitleText = "Test Subtitle 1", Streetcode = new StreetcodeContent { Id = 2 },
                StreetcodeId = 2,
            },
            new SubtitleEntity
            {
                Id = 3, SubtitleText = "Test Subtitle 3", Streetcode = new StreetcodeContent { Id = 4 },
                StreetcodeId = 4,
            },
        };

        var subtitleDto = new List<SubtitleDto>
        {
            new SubtitleDto
            {
                Id = 1, SubtitleText = "Test Subtitle 1", StreetcodeId = 2,
            },
            new SubtitleDto
            {
                Id = 3, SubtitleText = "Test Subtitle 3", StreetcodeId = 4,
            },
        };

        _repositoryWrapperMock.Setup(rock => rock.SubtitleRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<SubtitleEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<SubtitleEntity>,
                        IIncludableQueryable<SubtitleEntity, object>>>()))
            .ReturnsAsync(subtitle);

        _mapperMock.Setup(m => m.Map<IEnumerable<SubtitleDto>>(subtitle))
            .Returns(subtitleDto);

        // Act
        var result = await _handler.Handle(new GetAllSubtitlesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(subtitleDto, result.Value);
    }

    [Fact]
    public async Task Handler_ReturnsSubtitleDto_WhenSuntitleNotFound()
    {
        // Arrange
        const string exeptionErrorMsg = "Cannot find any subtitles";

        _repositoryWrapperMock.Setup(rock => rock.SubtitleRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<SubtitleEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<SubtitleEntity>,
                        IIncludableQueryable<SubtitleEntity, object>>>())) !
            .ReturnsAsync((IEnumerable<SubtitleEntity>)null);

        // Act
        var result = await _handler.Handle(new GetAllSubtitlesQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(exeptionErrorMsg, result.Errors[0].Message);
    }
}