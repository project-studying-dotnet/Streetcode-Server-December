using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

public class GetAllTagsHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetAllTagsHandler _handler;

    public GetAllTagsHandlerTest()
    {
        _handler = new GetAllTagsHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsAreFound()
    {
        // Arrange
        var tags = new List<TagEntity>
        {
            new TagEntity { Id = 1, Title = "Tag test 1", },
            new TagEntity { Id = 2, Title = "Tag test 2" },
        };

        var tagsDto = new List<TagDTO>
        {
            new TagDTO { Id = 1, Title = "Tag test 1" },
            new TagDTO { Id = 2, Title = "Tag test 2" },
        };

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.GetAllAsync(
            It.IsAny<Expression<Func<TagEntity, bool>>>(),
            It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()))
            .ReturnsAsync(tags);

        _mapperMock.Setup(map => map.Map<IEnumerable<TagDTO>>(tags))
            .Returns(tagsDto);

        // Act
        var result = await _handler.Handle(new GetAllTagsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tagsDto, result.Value);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsNotFound()
    {
        // Arrange
        const string errorMsg = $"Cannot find any tags";

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.GetAllAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()))
            .ReturnsAsync((IEnumerable<TagEntity>)null!);

        // Act
        var result = await _handler.Handle(new GetAllTagsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors[0].Message);

    }

}