using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streetcode.BLL.MediatR.Newss.Update;
using Xunit;
using Streetcode.BLL.DTO.News;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using FluentAssertions;
using System.Linq.Expressions;
using Streetcode.BLL.DTO.Media.Images;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.Domain.Entities.Media.Images;
using Streetcode.Domain.Entities.News;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class UpdateNewsTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;

        private readonly UpdateNewsHandler _handler;

        public UpdateNewsTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _blobServiceMock = new Mock<IBlobService>();

            _handler = new (
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenNewsIsNull()
        {
            // Arrange
            var command = new UpdateNewsCommand(new NewsDto());
            _mapperMock.Setup(m => m.Map<News>(It.IsAny<NewsDto>())).Returns((News)null);

            // Act
            var result = await _handler.Handle(command, default);

            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(command, "Cannot convert null to news"), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnSuccess_AndSetImage_WhenNewsImageIsNotNull()
        {
            // Arrange
            var news = new News
            {
                Id = 1,
                Title = "News 1",
                Image = new Image { BlobName = "test_image.jpg" },
            };

            var newsDTO = new NewsDto
            {
                Id = 1,
                Title = "News 1",
                Image = new ImageDto { BlobName = "test_image.jpg" },
            };

            var updatedNews = new NewsDto
            {
                Id = 1,
                Title = "News 1",
                Image = new ImageDto { BlobName = "base64string" },
            };

            var command = new UpdateNewsCommand(newsDTO);
            _mapperMock.Setup(m => m.Map<News>(newsDTO)).Returns(news);
            _mapperMock.Setup(m => m.Map<NewsDto>(news)).Returns(updatedNews);
            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("test_image.jpg"))
                .Returns(Task.FromResult("base64string"));

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Update(news));
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("base64string", result.Value.Image.BlobName);
            Assert.Equal(newsDTO.Id, result.Value.Id);
            Assert.Equal(newsDTO.Title, result.Value.Title);
        }

        [Fact]
        public async Task ShouldDeleteImage_WhenNewsImageIsNullAndImageExists()
        {
            // Arrange
            var news = new News { Id = 1, Title = "News 1", Image = null, ImageId = 1 };
            var newsDTO = new NewsDto { Id = 1, Title = "News 1", Image = null, ImageId = 1 };

            var command = new UpdateNewsCommand(newsDTO);
            _mapperMock.Setup(m => m.Map<News>(newsDTO)).Returns(news);
            _mapperMock.Setup(m => m.Map<NewsDto>(news)).Returns(newsDTO);
            _repositoryWrapperMock
                .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Image, bool>>>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(new Image());

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Update(news));
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(It.IsAny<Image>()), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var news = new News { Id = 1, Title = "News 1" };
            var newsDTO = new NewsDto { Id = 1, Title = "News 1" };

            var command = new UpdateNewsCommand(newsDTO);
            _mapperMock.Setup(m => m.Map<News>(newsDTO)).Returns(news);
            _mapperMock.Setup(m => m.Map<NewsDto>(news)).Returns(newsDTO);
            _repositoryWrapperMock
                .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Image, bool>>>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync((Image)null!);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Update(news));
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(command, "Failed to update a news"), Times.Once);
        }
    }
}
