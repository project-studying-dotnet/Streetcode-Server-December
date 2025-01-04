using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Streetcode.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ControllerTests.Streetcode.TextContent
{
    public class RelatedTermControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RelatedTermController _controller;

        public RelatedTermControllerTests()
        {
            this._mediatorMock = new Mock<IMediator>();

            this._controller = new RelatedTermController();

            var mediatorField = typeof(BaseApiController)
                .GetField("_mediator", BindingFlags.Instance | BindingFlags.NonPublic);

            mediatorField.SetValue(this._controller, this._mediatorMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsAllTerms()
        {
            // Arrange
            var expectedTerms = new List<RelatedTermDto>
            {
                new RelatedTermDto { Id = 1, Word = "TestTerm1", TermId = 1 },
                new RelatedTermDto { Id = 2, Word = "TestTerm2", TermId = 1 },
            };

            this._mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRelatedTermsQuery>(), default))
                              .ReturnsAsync(expectedTerms);

            // Act
            var result = await this._controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTerms, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsTermById()
        {
            // Arrange
            var termId = 1;
            var expectedTerm = new RelatedTermDto { Id = termId, Word = "TestTerm1", TermId = 1 };

            this._mediatorMock.Setup(m => m.Send(It.IsAny<GetRelatedTermByIdQuery>(), default))
                              .ReturnsAsync(expectedTerm);

            // Act
            var result = await this._controller.GetById(termId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTerm, okResult.Value);
        }

        [Fact]
        public async Task GetByTermId_ReturnsTermsByTermId()
        {
            // Arrange
            var termId = 1;
            var expectedTerms = new List<RelatedTermDto>
            {
                new RelatedTermDto { Id = 1, Word = "TestTerm1", TermId = termId },
                new RelatedTermDto { Id = 2, Word = "TestTerm2", TermId = termId },
            };

            this._mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRelatedTermsByTermIdQuery>(), default))
                              .ReturnsAsync(expectedTerms);

            // Act
            var result = await this._controller.GetByTermId(termId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTerms, okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult()
        {
            // Arrange
            var newTerm = new RelatedTermDto { Id = 3, Word = "NewTerm", TermId = 1 };
            this._mediatorMock.Setup(m => m.Send(It.IsAny<CreateRelatedTermCommand>(), default))
                              .ReturnsAsync(newTerm);

            // Act
            var result = await this._controller.Create(newTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(newTerm, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedResult()
        {
            // Arrange
            var updatedTerm = new RelatedTermDto { Id = 1, Word = "UpdatedTerm", TermId = 1 };
            this._mediatorMock.Setup(m => m.Send(It.IsAny<UpdateRelatedTermCommand>(), default))
                              .ReturnsAsync(updatedTerm);

            // Act
            var result = await this._controller.Update(updatedTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedTerm, okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsDeletedResult()
        {
            // Arrange
            var wordToDelete = "TestTerm";
            int termIdToDelete = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRelatedTermCommand>(), default))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Delete(wordToDelete, termIdToDelete);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_WhenHandlerFails_ReturnsBadRequest()
        {
            // Arrange
            var word = "nonexistentWord";
            var termId = 1;
            var command = new DeleteRelatedTermCommand(word, termId);

            _mediatorMock.Setup(m => m.Send(command, default))
                .ReturnsAsync(Result.Fail("Failed to delete related term"));

            // Act
            var result = await _controller.Delete(word, termId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
