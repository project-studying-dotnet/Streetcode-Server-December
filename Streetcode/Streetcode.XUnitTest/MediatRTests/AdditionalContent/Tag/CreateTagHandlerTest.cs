using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Xunit;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

public class CreateTagHandlerTest : AdditionalContentTestWrapper
{
    private readonly CreateTagHandler _handler;

    public CreateTagHandlerTest()
    {
        _handler = new CreateTagHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagAreCreate()
    {
        // Arrange
        var tag = new TagEntity { Title = "Tag test 1" };
        var tagDtoCreate = new CreateTagDto { Title = "Tag test 1" };
        var tagDto = new TagDto { Id = 1, Title = "Tag test 1" };
        var request = new CreateTagCommand(tagDtoCreate);

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.CreateAsync(It.IsAny<TagEntity>()))
            .ReturnsAsync(tag);
        _repositoryWrapperMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TagDto>(tag))
            .Returns(tagDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tagDto, result.Value);

        _repositoryWrapperMock.Verify(rep => rep.TagRepository.CreateAsync(It.IsAny<TagEntity>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TagDto>(tag), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenTagAreNotCreate()
    {
        // Arrange
        var request = new CreateTagCommand(new CreateTagDto { Title = "test" });

        _repositoryWrapperMock.Setup(rep => rep.TagRepository.CreateAsync(It.IsAny<TagEntity>()))
            .ReturnsAsync((TagEntity)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);

        _repositoryWrapperMock.Verify(rep => rep.TagRepository.CreateAsync(It.IsAny<TagEntity>()), Times.Once);
    }
}