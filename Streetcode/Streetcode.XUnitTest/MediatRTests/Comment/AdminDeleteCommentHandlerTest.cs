using FluentAssertions;
using MediatR;
using Moq;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Specifications;
using Xunit;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment
{
    public class AdminDeleteCommentHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly AdminDeleteCommentHandler _handler;

        public AdminDeleteCommentHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _handler = new AdminDeleteCommentHandler(_mockRepositoryWrapper.Object);
        }

        [Fact]
        public async Task ShouldThrowException_WhenRelatedTermNotFound()
        {
            // Arrange
            var command = new AdminDeleteCommentCommand(1);

            _mockRepositoryWrapper.Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<CommentEntity>>())).ReturnsAsync((CommentEntity)null!);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            exception.Message.Should().Be("Cannot find any comment");
        }

        [Fact]
        public async Task ShouldThrowException_WhenDeleteFails()
        {
            // Arrange
            var command = new AdminDeleteCommentCommand(1);
            var comment = new CommentEntity { Id = 1, UserName = "aa", UserFullName = "aaa", CreatedDate = DateTime.Now, Content = "aaaa", StreetcodeId = 1 };

            _mockRepositoryWrapper.Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<CommentEntity>>())).ReturnsAsync(comment);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            exception.Message.Should().Be("Failed to delete a comment");
        }

        [Fact]
        public async Task WhenDeleteSucceeds_ThenReturnDeletedRelatedTerm()
        {
            // Arrange
            var command = new AdminDeleteCommentCommand(1);
            var comment = new CommentEntity { Id = 1, UserName = "aa", UserFullName = "aaa", CreatedDate = DateTime.Now, Content = "aaaa", StreetcodeId = 1 };

            _mockRepositoryWrapper.Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<CommentEntity>>())).ReturnsAsync(comment);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(Unit.Value);
        }
    }
}
