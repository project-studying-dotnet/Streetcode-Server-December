using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.GetCommentsToReview;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment
{
    public class GetCommentsToReviewHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetCommentsToReviewHandler _handler;

        public GetCommentsToReviewHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetCommentsToReviewHandler(_mapperMock.Object, _repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenCommentsNotNull()
        {
            // Arrange
            var restrictedWords = new List<string> { "badword1", "badword2" };
            var query = new GetCommentsToReviewQuery(restrictedWords);

            var mockComments = new List<CommentEntity>
            {
                new CommentEntity { Content = "This contains badword1" },
                new CommentEntity { Content = "This contains badword2" }
            };
            var mockDtos = new List<GetCommentsToReviewDto>
            {
                new GetCommentsToReviewDto { Content = "This contains badword1" },
                new GetCommentsToReviewDto { Content = "This contains badword2" }
            };

            SetupGetAllAsync(mockComments);

            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<GetCommentsToReviewDto>>(mockComments))
                .Returns(mockDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mockDtos, result.Value);
        }

        [Fact]
        public async Task Handle_ReturnsFailResult_WhenCommentsAreNull()
        {
            // Arrange
            var restrictedWords = new List<string> { "badword1", "badword2" };
            var errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "comments");

            var query = new GetCommentsToReviewQuery(restrictedWords);

            SetupGetAllAsync(null!);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        private void SetupGetAllAsync(List<CommentEntity> comments)
        {
            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetAllAsync(
                It.IsAny<Expression<Func<CommentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>>()))
                .ReturnsAsync(comments);
        }
    }
}
