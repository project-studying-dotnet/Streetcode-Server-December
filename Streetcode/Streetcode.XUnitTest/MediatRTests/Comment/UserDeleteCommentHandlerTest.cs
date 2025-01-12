using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;
using Streetcode.BLL.MediatR.Comment.UserDeleteComment;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Specification;
using Xunit;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment;

public class UserDeleteCommentHandlerTest
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly UserDeleteCommentHandler _handler;

    public UserDeleteCommentHandlerTest()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _handler = new UserDeleteCommentHandler(_mockRepositoryWrapper.Object, _mockLoggerService.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCommentAreDelete()
    {
        // Arrange
        var userDeleteCommentDto = new UserDeleteCommentDto
        {
            Id = 1,
            UserName = "Test"
        };

        var comment = new CommentEntity
        {
            Id = 1,
            UserName = "Test"
        };
        
        var request = new UserDeleteCommentCommand(userDeleteCommentDto);
        
        _mockRepositoryWrapper.Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(
            It.IsAny<IBaseSpecification<CommentEntity>>()))
            .ReturnsAsync(comment);

        _mockRepositoryWrapper.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(1);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);

    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCommentAreNotFound()
    {
        // Arrange
        var userDeleteCommentDto = new UserDeleteCommentDto
        {
            Id = 1,
            UserName = "Test"
        };
        
        var request = new UserDeleteCommentCommand(userDeleteCommentDto);
        
        var errMsg = ErrorManager.GetCustomErrorText("CantFindError", "comment", request.UserDeleteCommentDto.Id);
        
        _mockRepositoryWrapper.Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<CommentEntity>>()))
            .ReturnsAsync((CommentEntity)null!);
        
        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
        
        // Assert
        exception.Message.Should().Be(errMsg);
        
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCommentAreNotDeleteFromDb()
    {
        // Arrange
        var userDeleteCommentDto = new UserDeleteCommentDto
        {
            Id = 1,
            UserName = "Test"
        };

        var comment = new CommentEntity
        {
            Id = 1,
            UserName = "Test"
        };
        
        var request = new UserDeleteCommentCommand(userDeleteCommentDto);
        
        var errMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "comment", $"comment {request.UserDeleteCommentDto.Id}");
        
        _mockRepositoryWrapper.Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<CommentEntity>>()))
            .ReturnsAsync(comment);
        
        _mockRepositoryWrapper.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(0);
        
        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
        
        // Assert
        exception.Message.Should().Be(errMsg);
        
    }

}