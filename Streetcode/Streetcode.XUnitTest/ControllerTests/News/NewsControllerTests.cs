using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MediatR;
using System.Threading.Tasks;
using Streetcode.WebApi.Controllers.News;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.BLL.MediatR.Newss.GetById;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.BLL.MediatR.Newss.Update;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;
using Streetcode.WebApi.Controllers;
using FluentResults;

namespace Streetcode.Tests.Controllers.News
{
    public class NewsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public NewsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new NewsController();

            var mediatorField = typeof(BaseApiController).GetField(
                "_mediator",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
            );

            mediatorField?.SetValue(_controller, _mediatorMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithNewsList()
        {
            // Arrange
            var newsList = new List<NewsDTO> { new NewsDTO { Id = 1, Title = "Test News" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllNewsQuery>(), default)).ReturnsAsync(newsList);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(newsList, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResultWithNews()
        {
            // Arrange
            var news = new NewsDTO { Id = 1, Title = "Test News" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewsByIdQuery>(), default)).ReturnsAsync(news);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(news, okResult.Value);
        }

        [Fact]
        public async Task GetByUrl_ReturnsOkResultWithNews()
        {
            // Arrange
            var news = new NewsDTO { Id = 1, Title = "Test News", URL = "test-url" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewsByUrlQuery>(), default)).ReturnsAsync(news);

            // Act
            var result = await _controller.GetByUrl("test-url");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(news, okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsOkResult()
        {
            // Arrange
            var news = new NewsDTO { Id = 1, Title = "Test News" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateNewsCommand>(), default)).ReturnsAsync(news);

            // Act
            var result = await _controller.Create(news);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(news, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOkResult()
        {
            // Arrange
            var news = new NewsDTO { Id = 1, Title = "Updated News" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateNewsCommand>(), default)).ReturnsAsync(news);

            // Act
            var result = await _controller.Update(news);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(news, okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOkObjectResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteNewsCommand>(), default))
                .ReturnsAsync(Unit.Value);

            // Act
            var actionResult = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Unit.Value, okResult.Value);
        }
    }
}
