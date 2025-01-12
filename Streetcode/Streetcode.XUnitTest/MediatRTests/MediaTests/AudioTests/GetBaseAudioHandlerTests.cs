using FluentResults;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class GetBaseAudioHandlerTests
    {
        private readonly Mock<IBlobService> _mockBlob;
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetBaseAudioHandler _getBaseAudioHandler;

        public GetBaseAudioHandlerTests()
        {
            _mockBlob = new Mock<IBlobService>();
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _getBaseAudioHandler = new GetBaseAudioHandler(_mockBlob.Object, _mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMemoryStream_WhenAudioExists()
        {
            // Arrange

            var audio = new Audio { Id = 1, BlobName = "audioBlob" };
            var expectedStream = new MemoryStream();

            _mockRepository.Setup(r => r.AudioRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null)).ReturnsAsync(audio);
            _mockBlob.Setup(b => b.FindFileInStorageAsMemoryStream(audio.BlobName)).Returns(Task.FromResult(expectedStream));

            // Act

            var result = await _getBaseAudioHandler.Handle(new GetBaseAudioQuery(audio.Id), CancellationToken.None);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedStream, result.Value);

            _mockRepository.Verify(r => r.AudioRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null), Times.Once);
            _mockBlob.Verify(b => b.FindFileInStorageAsMemoryStream(audio.BlobName), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenAudioDoesNotExist()
        {
            // Arrange

            int nonExistentAudioId = 99;
            _mockRepository.Setup(r => r.AudioRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null)).ReturnsAsync(null as Audio);

            // Act

            var result = await _getBaseAudioHandler.Handle(new GetBaseAudioQuery(nonExistentAudioId), CancellationToken.None);

            // Assert

            Assert.True(result.IsFailed);
            Assert.Single(result.Errors);
            Assert.Equal($"Cannot find a audio with corresponding id: {nonExistentAudioId}", result.Errors[0].Message);

            _mockLogger.Verify(l => l.LogError(It.IsAny<GetBaseAudioQuery>(), It.Is<string>(msg => msg.Contains($"id: {nonExistentAudioId}"))), Times.Once);
            _mockRepository.Verify(r => r.AudioRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null), Times.Once);
            _mockBlob.Verify(b => b.FindFileInStorageAsMemoryStream(It.IsAny<string>()), Times.Never);
        }
    }
}
