using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.Update;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Media.Audio;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;
namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class UpdateAudioHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly UpdateAudioHandler _updateAudioHandler;

        public UpdateAudioHandlerTests()
        {
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockBlobService = new Mock<IBlobService>();

            _updateAudioHandler = new UpdateAudioHandler(
                _mockMapper.Object,
                _mockRepository.Object,
                _mockBlobService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnSuccessfully_WhenUpdated()
        {
            // Arrange
            var command = SetupMocksForSuccessfulUpdate();

            // Act
            var result = await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ShouldFail_WhenAudioNotFound()
        {
            // Arrange
            var command = SetupMocksForAudioNotFound();

            // Act
            var result = await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("CannotFindAnAudioWithTheCorrespondingStreetcodeId", result.Errors.First().Message);
        }

        [Fact]
        public async Task ShouldFail_WhenBlobUpdateFails()
        {
            // Arrange
            var command = SetupMocksForBlobUpdateFailure();

            // Act
            var result = await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Blob storage error", result.Errors.First().Message);
            _mockLogger.Verify(logger => logger.LogError(
                It.IsAny<Exception>(),
                It.Is<string>(msg => msg.Contains("Error in UpdateAudioHandler"))), Times.Once);
        }

        [Fact]
        public async Task ShouldFail_WhenSaveChangesFails()
        {
            // Arrange
            var command = SetupMocksForSaveChangesFailure();

            // Act
            var result = await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("FailedToUpdateAudio", result.Errors.First().Message);
        }

        [Fact]
        public async Task ShouldMapUpdatedAudioCorrectly()
        {
            // Arrange
            var command = SetupMocksForSuccessfulUpdate();
            var expectedAudioDto = GetAudioDTO();

            // Act
            var result = await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedAudioDto.Base64, result.Value.Base64);
            Assert.Equal(expectedAudioDto.BlobName, result.Value.BlobName);
            Assert.Equal(expectedAudioDto.MimeType, result.Value.MimeType);
        }

        [Fact]
        public async Task ShouldLogError_WhenAudioNotFound()
        {
            // Arrange
            var command = SetupMocksForAudioNotFound();

            // Act
            await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(),
                It.Is<string>(msg => msg.Contains("CannotFindAnAudioWithTheCorrespondingStreetcodeId"))), Times.Once);
        }

        [Fact]
        public async Task ShouldNotCallBlobService_WhenAudioNotFound()
        {
            // Arrange
            var command = SetupMocksForAudioNotFound();

            // Act
            await _updateAudioHandler.Handle(command, CancellationToken.None);

            // Assert
            _mockBlobService.Verify(service => service.UpdateFileInStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        private UpdateAudioCommand SetupMocksForSuccessfulUpdate()
        {
            var testUpdateAudioDTO = GetUpdateAudioDTO();
            var testAudio = GetAudio();
            var testAudioDTO = GetAudioDTO();

            SetupUpdateRepositoryWithSpec(1, testAudio);
            SetupBlobService();
            SetupMapper(testAudio, testAudioDTO);

            return new UpdateAudioCommand(testUpdateAudioDTO);
        }

        private UpdateAudioCommand SetupMocksForAudioNotFound()
        {
            var testUpdateAudioDTO = GetUpdateAudioDTO();

            _mockRepository.Setup(repo => repo.AudioRepository
                .GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>()))
                .ReturnsAsync((AudioEntity)null!);

            return new UpdateAudioCommand(testUpdateAudioDTO);
        }

        private UpdateAudioCommand SetupMocksForBlobUpdateFailure()
        {
            var testUpdateAudioDTO = GetUpdateAudioDTO();
            var testAudio = GetAudio();

            SetupUpdateRepositoryWithSpec(1, testAudio);

            _mockBlobService.Setup(service => service.UpdateFileInStorage(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ThrowsAsync(new Exception("Blob storage error"));

            return new UpdateAudioCommand(testUpdateAudioDTO);
        }

        private UpdateAudioCommand SetupMocksForSaveChangesFailure()
        {
            var testUpdateAudioDTO = GetUpdateAudioDTO();
            var testAudio = GetAudio();

            SetupUpdateRepositoryWithSpec(0, testAudio);
            SetupBlobService();
            SetupMapper(testAudio, GetAudioDTO());

            return new UpdateAudioCommand(testUpdateAudioDTO);
        }

        private static AudioFileBaseUpdateDTO GetUpdateAudioDTO() => new()
        {
            Id = 1,
            Title = "Title",
            MimeType = "string",
            BaseFormat = "ab34",
            Extension = "string",
        };

        private static AudioDto GetAudioDTO() => new()
        {
            Id = 1,
            BlobName = "fake_blob_name",
            Base64 = "fake_base64_string",
            MimeType = "string",
        };

        private static AudioEntity GetAudio() => new()
        {
            Id = 1,
            BlobName = "hzbTZ58ebTjpDJDCWosy5F55WRkZU0cl+1Gpo_NWJ+0=.string",
            Base64 = "ab34",
            MimeType = "string",
        };

        private void SetupUpdateRepositoryWithSpec(int returnNumber, AudioEntity audioEntity)
        {
            _mockRepository.Setup(repo => repo.AudioRepository
                .GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>()))
                .ReturnsAsync(audioEntity);

            _mockRepository.Setup(x => x.AudioRepository.Update(It.IsAny<AudioEntity>()));
            _mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(returnNumber);
        }

        private void SetupBlobService()
        {
            _mockBlobService.Setup(service => service.UpdateFileInStorage(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync("fake_blob_name");

            _mockBlobService.Setup(service => service.FindFileInStorageAsBase64(
                It.IsAny<string>()))
                .ReturnsAsync("fake_base64_string");
        }

        private void SetupMapper(AudioEntity testAudio, AudioDto testAudioDTO)
        {
            _mockMapper.Setup(x => x.Map<AudioEntity>(It.IsAny<AudioFileBaseUpdateDTO>()))
                .Returns(testAudio);
            _mockMapper.Setup(x => x.Map<AudioDto>(It.IsAny<AudioEntity>()))
                .Returns(testAudioDTO);
        }
    }
}
