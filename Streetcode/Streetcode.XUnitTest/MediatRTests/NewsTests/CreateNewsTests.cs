using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Newss.Create;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class CreateNewsTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateNewsHandler _handler;

        public CreateNewsTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new CreateNewsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenNewsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateNewsCommand(new NewsDTO { Title = "Test News" });
            var newsEntity = new News { Title = "Test News" };
            var newsDto = new NewsDTO { Title = "Test News" };

            _mapperMock.Setup(m => m.Map<News>(command.newNews)).Returns(newsEntity);
            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(newsEntity)).Returns(newsEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity)).Returns(newsDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDto, result.Value);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenSaveChangesFails()
        {
            // Arrange
            var command = new CreateNewsCommand(new NewsDTO { Title = "Test News" });
            var newsEntity = new News { Title = "Test News" };

            _mapperMock.Setup(m => m.Map<News>(command.newNews)).Returns(newsEntity);
            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(newsEntity)).Returns(newsEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to create a news", result.Errors[0].Message);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenMappingReturnsNull()
        {
            // Arrange
            var command = new CreateNewsCommand(new NewsDTO { Title = "Test News" });

            _mapperMock.Setup(m => m.Map<News>(command.newNews)).Returns((News)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot convert null to news", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(command, "Cannot convert null to news"), Times.Once);
        }

        [Fact]
        public async Task ShouldSetImageIdToNull_WhenImageIdIsZero()
        {
            // Arrange
            var command = new CreateNewsCommand(new NewsDTO { Title = "Test News", ImageId = 0 });
            var newsEntity = new News { Title = "Test News", ImageId = 0 };
            News? createdNews = new News { Title = "Test News", ImageId = null };

            _mapperMock.Setup(m => m.Map<News>(command.newNews)).Returns(newsEntity);
            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(It.IsAny<News>())).Returns(createdNews);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<NewsDTO>(createdNews))
                .Returns(new NewsDTO { Title = "Test News", ImageId = null });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.Value.ImageId);
        }
    }
}
