using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.BLL.Specifications.Media.Audio;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class GetAudioByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAudioByIdHandler _handler;
        public GetAudioByIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new GetAudioByIdHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsAudioDto_WhenAudioExists()
        {
            // Arrange
            var audio = CreateAudio();
            var audioDto = CreateAudioDto(audio);
            var request = CreateGetAudioByIdQuery(audio.Id);

            SetupRepositoryToReturnAudio(audio);
            SetupMapperToReturnAudioDto(audio, audioDto);
            SetupBlobServiceToReturnBase64(audioDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertSuccessWithValue(result, audioDto);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenAudioNotFound()
        {
            // Arrange
            int id = 155;
            var request = CreateGetAudioByIdQuery(id);
            SetupRepositoryToReturnNull();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertFailureWithError(result, "Cannot find a audio with corresponding id");
            VerifyLoggerCalledWithError(request);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            var request = CreateGetAudioByIdQuery(id);
            var exceptionMessage = "Unexpected error";

            SetupRepositoryToThrowException(exceptionMessage);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertFailureWithError(result, exceptionMessage);
            VerifyLoggerCalledWithError(request, exceptionMessage);
        }

        [Fact]
        public async Task Handle_MapsAudioToAudioDtoCorrectly()
        {
            // Arrange
            var audio = CreateAudio();
            var audioDto = CreateAudioDto(audio);
            var request = CreateGetAudioByIdQuery(audio.Id);

            SetupRepositoryToReturnAudio(audio);
            SetupMapperToReturnAudioDto(audio, audioDto);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<AudioDto>(audio), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsBlobServiceWithCorrectBlobName()
        {
            // Arrange
            var audio = CreateAudio();
            var audioDto = CreateAudioDto(audio);
            var request = CreateGetAudioByIdQuery(audio.Id);

            SetupRepositoryToReturnAudio(audio);
            SetupMapperToReturnAudioDto(audio, audioDto);
            SetupBlobServiceToReturnBase64(audioDto);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64(audioDto.BlobName), Times.Once);
        }

        private Audio CreateAudio() => new Audio { Id = 1, BlobName = "blob123" };

        private AudioDto CreateAudioDto(Audio audio) => new AudioDto { BlobName = audio.BlobName, Base64 = "base64string" };

        private GetAudioByIdQuery CreateGetAudioByIdQuery(int id) => new GetAudioByIdQuery(id);

        private void SetupRepositoryToReturnAudio(Audio audio) =>
            _repositoryWrapperMock
                .Setup(r => r.AudioRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>()))
                .ReturnsAsync(audio);

        private void SetupRepositoryToReturnNull() =>
            _repositoryWrapperMock
                .Setup(r => r.AudioRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>()))
                .ReturnsAsync((Audio)null);

        private void SetupRepositoryToThrowException(string message) =>
            _repositoryWrapperMock
                .Setup(r => r.AudioRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<GetAudioByIdSpecification>()))
                .ThrowsAsync(new Exception(message));

        private void SetupMapperToReturnAudioDto(Audio audio, AudioDto audioDto) =>
            _mapperMock.Setup(m => m.Map<AudioDto>(audio)).Returns(audioDto);

        private void SetupBlobServiceToReturnBase64(AudioDto audioDto) =>
            _blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64(audioDto.BlobName))
                .ReturnsAsync(audioDto.Base64);

        private void AssertSuccessWithValue(Result<AudioDto> result, AudioDto expectedValue)
        {
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedValue, result.Value);
        }

        private void AssertFailureWithError(Result<AudioDto> result, string expectedError)
        {
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedError, result.Errors.First().Message);
        }

        private void VerifyLoggerCalledWithError(GetAudioByIdQuery request, string message = null)
        {
            if (message == null)
            {
                _mockLogger.Verify(l => l.LogError(request, It.IsAny<string>()), Times.Once);
            }
            else
            {
                _mockLogger.Verify(l => l.LogError(request, message), Times.Once);
            }
        }
    }
}
