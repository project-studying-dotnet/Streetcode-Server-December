using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.UpdateComment;
using Xunit;
using Streetcode.BLL.Resources;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Comment
{
    public class UpdateCommentHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly UpdateCommentHandler _handler;

        public UpdateCommentHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new UpdateCommentHandler(_mapperMock.Object, _repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnOkResultWhenCommentIsUpdated()
        {
            // Arrange
            var updateCommentDto = new UpdateCommentDto
            {
                StreetcodeId = 1,
                UserFullName = "Test User",
                CreatedDate = DateTime.Now.AddDays(-1),
                DateModified = DateTime.Now,
                Content = "Updated Content"
            };

            var commentEntity = new CommentEntity
            {
                StreetcodeId = 1,
                UserFullName = "Test User",
                CreatedDate = updateCommentDto.CreatedDate,
                Content = "Old Content"
            };

            var updatedComment = new CommentEntity
            {
                StreetcodeId = 1,
                UserFullName = "Test User",
                CreatedDate = updateCommentDto.CreatedDate,
                Content = updateCommentDto.Content,
                DateModified = updateCommentDto.DateModified
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1
            };

            var getCommentDto = new GetCommentDto
            {
                StreetcodeId = 1,
                Content = updateCommentDto.Content
            };

            var request = new UpdateCommentCommand(updateCommentDto);

            _repositoryMock.Setup(rep => rep.StreetcodeRepository
                    .GetFirstOrDefaultAsync(
                        It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcode)),
                        It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _mapperMock.Setup(m => m.Map<CommentEntity>(updateCommentDto)).Returns(updatedComment);

            _repositoryMock.Setup(rep => rep.CommentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<CommentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>>()))
            .ReturnsAsync(commentEntity);


            _repositoryMock.Setup(rep => rep.SaveChangesAsync()).ReturnsAsync(1);

            _mapperMock.Setup(m => m.Map<GetCommentDto>(updatedComment)).Returns(getCommentDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(updatedComment.Content, commentEntity.Content);
            Assert.Equal(updatedComment.CreatedDate, commentEntity.CreatedDate);
        }

        [Fact]
        public async Task Handle_ReturnFailResult_WhenStreetcodeNotFound()
        {
            // Arrange
            var updateCommentDto = new UpdateCommentDto
            {
                StreetcodeId = 1
            };

            var request = new UpdateCommentCommand(updateCommentDto);

            var errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "streetcode", updateCommentDto.StreetcodeId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnFailResultWhenCommentNotMapped()
        {
            // Arrange
            var updateCommentDto = new UpdateCommentDto
            {
                StreetcodeId = 1
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1
            };

            var request = new UpdateCommentCommand(updateCommentDto);

            var errMsg = ErrorManager.GetCustomErrorText("ConvertationError", "UpdateCommandDto", "Comment");

            _repositoryMock.Setup(rep => rep.StreetcodeRepository
                    .GetFirstOrDefaultAsync(
                        It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcode)),
                        It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _mapperMock.Setup(m => m.Map<CommentEntity>(updateCommentDto)).Returns((CommentEntity)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errMsg, result.Errors[0].Message);
        }
    }
}
