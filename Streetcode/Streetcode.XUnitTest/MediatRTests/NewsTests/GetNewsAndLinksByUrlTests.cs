using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.BLL.DTO.Media.Images;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Security.Policy;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class GetNewsAndLinksByUrlTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly GetNewsAndLinksByUrlHandler _handler;

        public GetNewsAndLinksByUrlTests()
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
            var query = new GetNewsAndLinksByUrlQuery("test-url.com");

            _repositoryWrapperMock
                 .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(), null))
                 .ReturnsAsync((News)null);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(query, "No news by entered Url - test-url.com"), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnCorrectLinks_WhenFoundMultipleNews()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News { Id = 1, URL = "news-1", Title = "News 1", Image = null },
                new News { Id = 2, URL = "news-2", Title = "News 2", Image = null },
                new News { Id = 3, URL = "news-3", Title = "News 3", Image = null },
            };
            var currentNews = newsList[1];
            var newsDTO = new NewsDTO { Id = 2, Title = "News 2", Image = null };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(currentNews);

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(newsList);

            _mapperMock.Setup(m => m.Map<NewsDTO>(currentNews)).Returns(newsDTO);

            var query = new GetNewsAndLinksByUrlQuery("news-2");

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("news-1", result.Value.PrevNewsUrl);
            Assert.Equal("news-3", result.Value.NextNewsUrl);
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

            var newsDTO = new NewsDTO
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

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(new List<News> { news });

            _mapperMock
                .Setup(m => m.Map<NewsDTO>(news))
                .Returns(newsDTO);

            var query = new GetNewsAndLinksByUrlQuery("test-url.com");

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTO.Title, result.Value.News.Title);
            Assert.Null(result.Value.News.Image);
            Assert.Null(result.Value.PrevNewsUrl);
            Assert.Null(result.Value.NextNewsUrl);

            _blobServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenNewsFoundWithImage_InvokeSBlobService()
        {
            // Arrange
            var news = new News
            {
                Id = 1,
                Title = "News 1",
                Image = new Image { BlobName = "test_image.jpg" },
                URL = "test - url.com"
            };

            var newsDTO = new NewsDTO
            {
                Id = 1,
                Title = "News 1",
                Image = new ImageDTO { BlobName = "test_image.jpg" },
                URL = "test - url.com"
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(new List<News> { news });

            _mapperMock.Setup(m => m.Map<NewsDTO>(news)).Returns(newsDTO);

            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("test_image.jpg"))
                .Returns("base64string");

            var query = new GetNewsAndLinksByUrlQuery("test-url.com");

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTO.URL, result.Value.News.URL);
            Assert.NotNull(result.Value.News.Image);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test_image.jpg"), Times.Once);
        }
    }
}
