using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.CreateReply;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment
{
    public class CreateReplyHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateReplyHandler _handler;

        public CreateReplyHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new CreateReplyHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenParentCommentNotFound()
        {
            // Arrange
            var command = new CreateReplyCommand(new CreateReplyDto { ParentId = 1 }, "John Doe");
            _repositoryWrapperMock
                .Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentByParentIdSpecification>()))
                .ReturnsAsync((CommentEntity)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            // Verify that logger was called with the specific error message
            _loggerMock.Verify(l => l.LogError(It.IsAny<CreateReplyCommand>(), It.Is<string>(msg => msg.Contains("Cannot find any comment"))), Times.Once);

            // Verify that logger was not called for other issues in this scenario
            _loggerMock.Verify(l => l.LogError(It.IsAny<CreateReplyCommand>(), It.Is<string>(msg => msg.Contains("Cannot convert create reply dto to CommentEntity"))), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenMappingFails()
        {
            // Arrange
            var command = new CreateReplyCommand(new CreateReplyDto { ParentId = 1 }, "John Doe");
            _repositoryWrapperMock
                .Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentByParentIdSpecification>()))
                .ReturnsAsync(new CommentEntity());
            _mapperMock
                .Setup(m => m.Map<CommentEntity>(It.IsAny<CreateReplyDto>()))
                .Returns((CommentEntity)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(l => l.LogError(It.IsAny<CreateReplyCommand>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenSaveChangesFails()
        {
            // Arrange
            var command = new CreateReplyCommand(new CreateReplyDto { ParentId = 1 }, "John Doe");
            var replyEntity = new CommentEntity();

            _repositoryWrapperMock
                .Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentByParentIdSpecification>()))
                .ReturnsAsync(new CommentEntity());
            _mapperMock
                .Setup(m => m.Map<CommentEntity>(It.IsAny<CreateReplyDto>()))
                .Returns(replyEntity);
            _repositoryWrapperMock
                .Setup(r => r.CommentRepository.CreateAsync(It.IsAny<CommentEntity>()))
                .ReturnsAsync(replyEntity);
            _repositoryWrapperMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(l => l.LogError(It.IsAny<CreateReplyCommand>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenReplyIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateReplyCommand(new CreateReplyDto { ParentId = 1 }, "John Doe");
            var replyEntity = new CommentEntity();
            var replyDto = new CreateReplyDto();

            _repositoryWrapperMock
                .Setup(r => r.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentByParentIdSpecification>()))
                .ReturnsAsync(new CommentEntity());
            _mapperMock
                .Setup(m => m.Map<CommentEntity>(It.IsAny<CreateReplyDto>()))
                .Returns(replyEntity);
            _repositoryWrapperMock
                .Setup(r => r.CommentRepository.CreateAsync(It.IsAny<CommentEntity>()))
                .ReturnsAsync(replyEntity);
            _repositoryWrapperMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mapperMock
                .Setup(m => m.Map<CreateReplyDto>(It.IsAny<CommentEntity>()))
                .Returns(replyDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(replyDto, result.Value);
        }
    }
}
