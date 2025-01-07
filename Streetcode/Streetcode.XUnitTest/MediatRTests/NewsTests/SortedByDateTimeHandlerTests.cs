using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.News;
using System.Linq.Expressions;
using Xunit;
using System.Collections;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class SortedByDateTimeHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly SortedByDateTimeHandler _handler;

        public SortedByDateTimeHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new (
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenNewsNotFound()
        {
            // Arrange
            var query = new SortedByDateTimeQuery();

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync((IEnumerable<News>)null);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Errors);
            _loggerMock.Verify(log => log.LogError(query, "Cannot find any news"), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnSortedNews_WhenNewsExists()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News
                {
                    Id = 1,
                    Title = "News 1",
                    Image = null,
                    CreationDate = DateTime.Now.AddDays(-1),
                },
                new News
                {
                    Id = 2,
                    Title = "News 2",
                    Image = null,
                    CreationDate = DateTime.Now,
                },
                new News
                {
                    Id = 3,
                    Title = "News 3",
                    Image = null,
                    CreationDate = DateTime.Now.AddDays(-2),
                },
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(newsList);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<NewsDto>>(newsList))
                .Returns((IEnumerable<News> source) =>
                source.Select(n => new NewsDto { Id = n.Id, Title = n.Title, CreationDate = n.CreationDate }));

            var query = new SortedByDateTimeQuery();

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal("News 2", result.Value.First().Title); // Most recent news
            Assert.Equal("News 3", result.Value.Last().Title);  // Oldest news
        }

        [Fact]
        public async Task ShouldReturnSortedNews_AndSetImage_WhenNewsHasImage()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News
                {
                    Id = 1,
                    Title = "News 1",
                    Image = new Image { BlobName = "test_image.jpg" },
                    CreationDate = DateTime.Now,
                },
                new News
                {
                    Id = 2,
                    Title = "News 2",
                    Image = new Image { BlobName = "test_image.jpg" },
                    CreationDate = DateTime.Now.AddDays(-1),
                },
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(newsList);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<NewsDto>>(newsList))
                .Returns((IEnumerable<News> source) => source.Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    CreationDate = n.CreationDate,
                    Image = new ImageDto { BlobName = n.Image.BlobName }
                }));

            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("test_image.jpg"))
                .Returns(Task.FromResult("base64string"));

            var query = new SortedByDateTimeQuery();

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.All(result.Value, item => Assert.NotNull(item.Image));
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test_image.jpg"), Times.Exactly(2));
        }
    }
}