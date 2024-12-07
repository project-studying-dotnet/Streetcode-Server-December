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
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.DAL.Entities.Media.Images;
using System.Linq.Expressions;

namespace Streetcode.XUnitTest.MediatRTests.NewsTests
{
    public class DeleteNewsTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly DeleteNewsHandler _handler;

        public DeleteNewsTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new DeleteNewsHandler(_repositoryWrapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldReturnFailResult_WhenNewsNotFound_()
        {
            // Arrange
            var command = new DeleteNewsCommand(1);
            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(),null))
                .ReturnsAsync((News)null);

            // Act
            var result = await _handler.Handle(command, default);
            
            // Assert
            Assert.False(result.IsSuccess);
            _loggerMock.Verify(log => log.LogError(command, "No news found by entered Id - 1"), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSuccessResult_NewsFoundWithImage_DeletesNewsAndImage()
        {
            // Arrange
            var command = new DeleteNewsCommand(1);
            var news = new News { Id = 1, Image = new () };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(), null))
                .ReturnsAsync(news);
            _repositoryWrapperMock.Setup(repo => repo.ImageRepository.Delete(news.Image));
            _repositoryWrapperMock.Setup(repo => repo.NewsRepository.Delete(news));
            _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(news.Image), Times.Once);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Delete(news), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSuccessResult_NewsFoundWithoutImage_DeletesNewsOnly()
        {
            // Arrange
            var command = new DeleteNewsCommand(1);
            var news = new News { Id = 1, Image = null };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(), null))
                .ReturnsAsync(news);
            _repositoryWrapperMock.Setup(repo => repo.NewsRepository.Delete(news));
            _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(It.IsAny<Image>()), Times.Never);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Delete(news), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailResult_WhenSaveChangesFails()
        {
            // Arrange
            var command = new DeleteNewsCommand(1);
            var news = new News { Id = 1 };

            _repositoryWrapperMock
                .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<News, bool>>>(), null))
                .ReturnsAsync(news);
            _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            _loggerMock.Verify(log => log.LogError(command, "Failed to delete news"), Times.Once);
        }
    }
}
