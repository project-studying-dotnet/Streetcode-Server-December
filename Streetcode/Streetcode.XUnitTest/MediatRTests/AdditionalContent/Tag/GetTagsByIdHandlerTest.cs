using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Xunit;
using TagEntity = Streetcode.Domain.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

public class GetTagsByIdHandlerTest : AdditionalContentTestWrapper
{
    private readonly GetTagByIdHandler _handler;

    public GetTagsByIdHandlerTest()
    {
        _handler = new GetTagByIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsByIdAreFound()
    {
        // Arrange
        var tagId = 1;
        var request = new GetTagByIdQuery(tagId);
        var tag = new TagEntity { Id = 1, Title = "Test tag 1" };
        var tagDto = new TagDto { Id = 1, Title = "Tag test 1" };

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<TagEntity, bool>>>(exp => exp.Compile().Invoke(tag)),
                It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()))
            .ReturnsAsync(tag);

        _mapperMock.Setup(m => m.Map<TagDto>(tag))
            .Returns(tagDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tagDto, result.Value);

        _repositoryWrapperMock.Verify(
            rep => rep.TagRepository.GetFirstOrDefaultAsync(
            It.Is<Expression<Func<TagEntity, bool>>>(exp => exp.Compile().Invoke(new TagEntity { Id = request.Id })),
            It.IsAny<Func<IQueryable<TagEntity>, IIncludableQueryable<TagEntity, object>>>()), Times.Once);

        _mapperMock.Verify(m => m.Map<TagDto>(tag), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagsByIdNotFound()
    {
        // Arrange
        var tagId = 1;
        var request = new GetTagByIdQuery(tagId);
        string errorMsg = $"Cannot find a tag with corresponding id: {request.Id}";

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