using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.BLL.DTO.Media.Images;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class GetAllNewsTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly GetAllNewsHandler _handler;

        public GetAllNewsTests()
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
        public async Task ShouldReturnFail_WhenNewsNotFound()
        {
            // Arrange
            var query = new GetAllNewsQuery();

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync((IEnumerable<News>)null);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(query, "There are no news in the database"), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnsSuccess_WhenNewsFoundWithImages()
        {
            // Arrange
            var news = new List<News>
            {
                new News { Id = 1, Title = "News 1", Image = new Image { BlobName = "test_image.jpg" } },
                new News { Id = 2, Title = "News 2", Image = new Image { BlobName = "test_image.jpg" } }
            };

            var newsDTOs = new List<NewsDTO>
            {
                new NewsDTO { Id = 1, Title = "News 1", Image = new ImageDTO { BlobName = "test_image.jpg" } },
                new NewsDTO { Id = 2, Title = "News 2", Image = new ImageDTO { BlobName = "test_image.jpg" }}
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<NewsDTO>>(news))
                .Returns(newsDTOs);

            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("test_image.jpg"))
                .Returns("base64string");

            var query = new GetAllNewsQuery();

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTOs, result.Value);
            Assert.Equal(2, result.Value.Count());
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test_image.jpg"), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldReturnsSuccess_WhenNewsFoundWithoutImages_DoesNotInvokeBlobService()
        {
            // Arrange
            var news = new List<News>
            {
                new News { Id = 1, Title = "News 1", Image = null },
                new News { Id = 2, Title = "News 2", Image = null }
            };

            var newsDTOs = new List<NewsDTO>
            {
                new NewsDTO { Id = 1, Title = "News 1", Image = null },
                new NewsDTO { Id = 2, Title = "News 2", Image = null }
            };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetAllAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<NewsDTO>>(news))
                .Returns(newsDTOs);

            var query = new GetAllNewsQuery();

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
            Assert.All(result.Value, item => Assert.Null(item.Image));
            _blobServiceMock.VerifyNoOtherCalls();
        }
    }
}
