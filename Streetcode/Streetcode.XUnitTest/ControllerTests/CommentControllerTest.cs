using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.WebApi.Controllers.Streetcode.TextContent;
using Streetcode.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.WebApi.Controllers.Comment;
using FluentResults;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;
using FluentAssertions;

namespace Streetcode.XUnitTest.ControllerTests
{
    public class CommentControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CommentController _controller;

        public CommentControllerTest()
        {
            this._mediatorMock = new Mock<IMediator>();

            this._controller = new CommentController();

            var mediatorField = typeof(BaseApiController)
                .GetField("_mediator", BindingFlags.Instance | BindingFlags.NonPublic);

            mediatorField.SetValue(this._controller, this._mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllCommentsByStreetcodeId_ReturnsAllComments()
        {
            // Arrange
            var mockDtos = new List<GetCommentDto>
            {
                new GetCommentDto { Id = 1, StreetcodeId = 1, Content = "Test Comment 1" },
                new GetCommentDto { Id = 2, StreetcodeId = 1, Content = "Test Comment 2" }
            };

            this._mediatorMock.Setup(m => m.Send(It.IsAny<GetCommentsByStreetcodeIdQuery>(), default))
                              .ReturnsAsync(mockDtos);

            // Act
            var result = await this._controller.GetByStreetcodeId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockDtos, okResult.Value);
        }

        [Fact]
        public async Task AdminDeleteComment_ReturnsOkResult_WithValidCommentId()
        {
            // Arrange
            int commentId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AdminDeleteCommentCommand>(), default))
                .ReturnsAsync(Result.Ok());

            // Act
            var result = await _controller.AdminDeleteComment(commentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AdminDeleteComment_ReturnsBadRequest_WhenDeletionFails()
        {
            // Arrange
            int commentId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AdminDeleteCommentCommand>(), default))
                .ReturnsAsync(Result.Fail("Deletion failed"));

            // Act
            var result = await _controller.AdminDeleteComment(commentId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
