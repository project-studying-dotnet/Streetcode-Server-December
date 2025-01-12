using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.DAL.Entities.Media.Images;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Security.Policy;
using Streetcode.Domain.Entities.News;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class GetNewsByUrlTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly GetNewsByUrlHandler _handler;

        public GetNewsByUrlTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new (
                _mapperMock.Object,
                _repositoryWrapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenNewsNotFound()
        {
            // Arrange
            var query = new GetNewsByUrlQuery("test-url.com");

            _repositoryWrapperMock
                 .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(), null))
                 .ReturnsAsync((News)null);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(query, "Cannot find any news by Url: test-url.com"), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenNewsFoundWithImage()
        {
            // Arrange
            var news = new News
            {
                Id = 1,
                Title = "News 1",
                Image = new Image { BlobName = "test_image.jpg" },
                URL = "test - url.com",
            };

            var newsDTO = new NewsDto
            {
                Id = 1,
                Title = "News 1",
                Image = new ImageDto { BlobName = "test_image.jpg" },
                URL = "test - url.com",
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _mapperMock.Setup(m => m.Map<NewsDto>(news)).Returns(newsDTO);

            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("test_image.jpg"))
                .Returns(Task.FromResult("base64string"));

            var query = new GetNewsByUrlQuery("test-url.com");

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTO.URL, result.Value.URL);
            Assert.NotNull(result.Value.Image);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test_image.jpg"), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenNewsFoundWithoutImage_DoesNotInvokeBlobService()
        {
            // Arrange
            var news = new News
            {
                Id = 1,
                Title = "News 1",
                Image = null,
                URL = "test - url.com",
            };

            var newsDTO = new NewsDto
            {
                Id = 1,
                Title = "News 1",
                Image = null,
                URL = "test - url.com",
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _mapperMock
                .Setup(m => m.Map<NewsDto>(news))
                .Returns(newsDTO);

            var query = new GetNewsByUrlQuery("test-url.com");

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTO.URL, result.Value.URL);
            Assert.Null(result.Value.Image);

            _blobServiceMock.VerifyNoOtherCalls();
        }
    }
}
