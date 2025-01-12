using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.Specifications.Media.Audio;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class DeleteAudioHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<IBlobService> _mockBlob;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly DeleteAudioHandler _deleteAudioHandler;

        public DeleteAudioHandlerTests()
        {
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockBlob = new Mock<IBlobService>();
            _mockLogger = new Mock<ILoggerService>();
            _deleteAudioHandler = new DeleteAudioHandler(_mockRepository.Object, _mockBlob.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAudioSuccessfully_WhenAudioExists()
        {
            // Arrange

            var audioId = 1;
            var audio = new Audio { Id = audioId, BlobName = "blob1" };

            _mockRepository.Setup(r => r.AudioRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>())).ReturnsAsync(audio);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act

            var result = await _deleteAudioHandler.Handle(new DeleteAudioCommand(audioId), CancellationToken.None);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Equal(Unit.Value, result.Value);

            _mockRepository.Verify(r => r.AudioRepository.Delete(audio), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockBlob.Verify(b => b.DeleteFileInStorage(audio.BlobName), Times.Once);
            _mockLogger.Verify(l => l.LogInformation("DeleteAudioCommand handled successfully"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenAudioDoesNotExist()
        {
            // Arrange

            var audioId = 123;

            _mockRepository.Setup(r => r.AudioRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>())).ReturnsAsync(null as Audio);

            // Act

            var result = await _deleteAudioHandler.Handle(new DeleteAudioCommand(audioId), CancellationToken.None);

            // Assert

            Assert.True(result.IsFailed);
            Assert.Single(result.Errors);
            Assert.Equal($"Cannot find a audio with corresponding id: {audioId}", result.Errors[0].Message);

            _mockLogger.Verify(l => l.LogError(new DeleteAudioCommand(audioId), $"Cannot find a audio with corresponding id: {audioId}"), Times.Once);

            _mockRepository.Verify(r => r.AudioRepository.Delete(It.IsAny<Audio>()), Times.Never);
            _mockBlob.Verify(b => b.DeleteFileInStorage(It.IsAny<string>()), Times.Never);
        }
    }
}
