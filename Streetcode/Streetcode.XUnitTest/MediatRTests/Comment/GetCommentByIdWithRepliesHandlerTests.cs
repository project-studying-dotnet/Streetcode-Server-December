using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Comment.GetCommentByIdWithReplies;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment
{
    public class GetCommentByIdWithRepliesHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetCommentByIdWithRepliesHandler _handler;

        public GetCommentByIdWithRepliesHandlerTests()
        {
            _repositoryMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetCommentByIdWithRepliesHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_CommentExists_ReturnsCommentDto()
        {
            // Arrange
            var commentId = 1;
            var commentEntity = new CommentEntity { Id = commentId, Content = "Test Comment" };
            var commentDto = new GetCommentDto { Id = commentId, Content = "Test Comment" };

            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentWithChildrenSpecification>()))
                .ReturnsAsync(commentEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<GetCommentDto>(commentEntity))
                .Returns(commentDto);

            var request = new GetCommentByIdWithRepliesQuery(commentId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(commentDto.Id, result.Value.Id);
            Assert.Equal(commentDto.Content, result.Value.Content);
        }

        [Fact]
        public async Task Handle_CommentDoesNotExist_ThrowsException()
        {
            // Arrange
            var commentId = 1;
            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentWithChildrenSpecification>()))
                .ReturnsAsync((CommentEntity)null);

            var request = new GetCommentByIdWithRepliesQuery(commentId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Contains("Cannot find any comment", exception.Message);
        }

        [Fact]
        public async Task Handle_MapperReturnsNull_ThrowsException()
        {
            // Arrange
            var commentId = 1;
            var commentEntity = new CommentEntity { Id = commentId, Content = "Sample Comment" };

            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<CommentWithChildrenSpecification>()))
                .ReturnsAsync(commentEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<GetCommentDto>(commentEntity))
                .Returns((GetCommentDto)null);

            var request = new GetCommentByIdWithRepliesQuery(commentId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Contains("Cannot convert comment to commentDto", exception.Message);
        }
    }
}
