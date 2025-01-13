using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.BLL.MediatR.Newss.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class GetNewsByIdTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly GetNewsByIdHandler _handler;

        public GetNewsByIdTests()
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
            var query = new GetNewsByIdQuery(1);

            _repositoryWrapperMock
                 .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(), null))
                 .ReturnsAsync((News)null);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(query, "Cannot find a news with corresponding id: 1"), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenNewsFoundWithImage()
        {
            // Arrange
            var news = new News { Id = 1, Title = "News 1", Image = new Image { BlobName = "test_image.jpg" } };

            var newsDTO = new NewsDto { Id = 1, Title = "News 1", Image = new ImageDto { BlobName = "test_image.jpg" } };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _mapperMock
                .Setup(m => m.Map<NewsDto>(news))
                .Returns(newsDTO);

            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("test_image.jpg"))
                .Returns(Task.FromResult("base64string"));

            var query = new GetNewsByIdQuery(1);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTO, result.Value);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test_image.jpg"), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenNewsFoundWithoutImage_DoesNotInvokeBlobService()
        {
            // Arrange
            var news = new News { Id = 1, Title = "News 1", Image = null };

            var newsDTO = new NewsDto { Id = 1, Title = "News 1", Image = null };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<News, bool>>>(),
                    It.IsAny<Func<IQueryable<News>, IIncludableQueryable<News, object>>>()))
                .ReturnsAsync(news);

            _mapperMock
                .Setup(m => m.Map<NewsDto>(news))
                .Returns(newsDTO);

            var query = new GetNewsByIdQuery(1);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value.Image);

            _blobServiceMock.VerifyNoOtherCalls();
        }
    }
}
