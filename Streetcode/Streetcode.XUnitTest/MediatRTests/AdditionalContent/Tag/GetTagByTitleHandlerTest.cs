using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetTagByTitle;
using Xunit;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

public class GetTagByTitleHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetTagByTitleHandler _handler;

    public GetTagByTitleHandlerTest()
    {
        _handler = new GetTagByTitleHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsByTitleAreFound()
    {
        const string tagTitle = "Tag test 1";
        var request = new GetTagByTitleQuery(tagTitle);
        var tag = new TagEntity { Id = 1, Title = "Tag test 1" };
        var tagDto = new TagDTO { Id = 1, Title = "Tag test 1" };

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<TagEntity, bool>>>(exp => exp.Compile().Invoke(tag)),
                It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()))
            .ReturnsAsync(tag);

        _mapperMock.Setup(m => m.Map<TagDTO>(tag))
            .Returns(tagDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tagDto, result.Value);

        _repositoryWrapperMock.Verify(
            rep => rep.TagRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<TagEntity, bool>>>(
                    exp => exp.Compile().Invoke(new TagEntity { Title = request.Title})),
                It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()), Times.Once);

        _mapperMock.Verify(m => m.Map<TagDTO>(tag), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsByTitleNotFound()
    {
        const string tagTitle = "Tag test 1";
        var request = new GetTagByTitleQuery(tagTitle);
        string errorMsg = $"Cannot find any tag by the title: {request.Title}";

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()))
            .ReturnsAsync((TagEntity)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors[0].Message);
    }

}