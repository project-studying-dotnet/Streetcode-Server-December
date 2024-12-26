using AutoMapper;
using FluentResults;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment
{
    public class GetCommentsByStreetcodeIdHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetCommentsByStreetcodeIdHandler _handler;

        public GetCommentsByStreetcodeIdHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetCommentsByStreetcodeIdHandler(
                _mapperMock.Object,
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenCommentsExist()
        {
            // Arrange
            var streetcodeId = 123;
            var request = new GetCommentsByStreetcodeIdQuery(streetcodeId);

            var mockComments = new List<CommentEntity>
        {
            new CommentEntity { Id = 1, StreetcodeId = streetcodeId, Content = "Test Comment 1" },
            new CommentEntity { Id = 2, StreetcodeId = streetcodeId, Content = "Test Comment 2" }
        };

            var mockDtos = new List<GetCommentDto>
        {
            new GetCommentDto { Id = 1, StreetcodeId = streetcodeId, Content = "Test Comment 1" },
            new GetCommentDto { Id = 2, StreetcodeId = streetcodeId, Content = "Test Comment 2" }
        };

            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetAllBySpecAsync(It.IsAny<CommentByStreetcodeIdSpecification>()))
                .ReturnsAsync(mockComments);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<GetCommentDto>>(mockComments))
                .Returns(mockDtos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task Handle_ReturnsErrorResult_WhenCommentsNotFound()
        {
            // Arrange
            var streetcodeId = 123;
            var request = new GetCommentsByStreetcodeIdQuery(streetcodeId);

            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetAllBySpecAsync(It.IsAny<CommentByStreetcodeIdSpecification>()))
                .ReturnsAsync((IEnumerable<CommentEntity>)null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error with Message='Cannot find a comment by a streetcode id: 123'", result.Errors[0].ToString());
            Assert.IsAssignableFrom<Result<IEnumerable<GetCommentDto>>>(result);
        }

        [Fact]
        public async Task Handle_ReturnsErrorResult_WhenMappingFails()
        {
            // Arrange
            var streetcodeId = 123;
            var request = new GetCommentsByStreetcodeIdQuery(streetcodeId);

            var mockComments = new List<CommentEntity>
        {
            new CommentEntity { Id = 1, StreetcodeId = streetcodeId, Content = "Test Comment 1" },
            new CommentEntity { Id = 2, StreetcodeId = streetcodeId, Content = "Test Comment 2" }
        };

            _repositoryMock
                .Setup(repo => repo.CommentRepository.GetAllBySpecAsync(It.IsAny<CommentByStreetcodeIdSpecification>()))
                .ReturnsAsync(mockComments);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<GetCommentDto>>(mockComments))
                .Returns((IEnumerable<GetCommentDto>)null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error with Message='Cannot find any DTOS for comments'", result.Errors[0].ToString());
            Assert.IsAssignableFrom<Result<IEnumerable<GetCommentDto>>>(result);
        }
    }
}
