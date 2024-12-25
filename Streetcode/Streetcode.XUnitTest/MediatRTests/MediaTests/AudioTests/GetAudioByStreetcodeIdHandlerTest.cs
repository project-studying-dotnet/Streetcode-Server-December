using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Streetcode;
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
    public class GetAudioByStreetcodeIdHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAudioByStreetcodeIdHandler _handler;

        public GetAudioByStreetcodeIdHandlerTest()
        {
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockBlobService = new Mock<IBlobService>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new GetAudioByStreetcodeIdHandler(_mockRepository.Object, _mockMapper.Object, _mockBlobService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAudioDTO_WhenStreetcodeAndAudioExist()
        {
            // Arrange
            var query = new GetAudioByStreetcodeIdQuery(1);

            var audio = new Audio { Id = 1, BlobName = "audioBlob" };
            var streetcode = new StreetcodeContent { Id = 1, Index = 1, Audio = audio };
            var audioDto = new AudioDTO { BlobName = "audioBlob", Base64 = "base64string" };

            _mockRepository.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _mockMapper.Setup(m => m.Map<AudioDTO>(audio)).Returns(audioDto);
            _mockBlobService.Setup(b => b.FindFileInStorageAsBase64(audio.BlobName)).Returns("base64string");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("audioBlob", result.Value.BlobName);
            Assert.Equal("base64string", result.Value.Base64);

            _mockRepository.Verify(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()), Times.Once);

            _mockMapper.Verify(m => m.Map<AudioDTO>(audio), Times.Once);
            _mockBlobService.Verify(b => b.FindFileInStorageAsBase64(audio.BlobName), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNullResult_WhenAudioIsNull()
        {
            // Arrange
            var query = new GetAudioByStreetcodeIdQuery(1);

            var audio = new Audio { Id = 1, BlobName = "audioBlob" };
            var streetcode = new StreetcodeContent { Id = 1, Index = 1 };

            _mockRepository.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnAudioDto_WhenAudioExists()
        {
            // Arrange
            var query = new GetAudioByStreetcodeIdQuery(1);

            var audio = new Audio { Id = 1, BlobName = "audioBlob" };
            var streetcode = new StreetcodeContent { Id = 1, Index = 1, Audio = audio };
            var audioDto = new AudioDTO { BlobName = "audioBlob", Base64 = "base64string" };

            _mockRepository.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _mockMapper.Setup(m => m.Map<AudioDTO>(audio)).Returns(audioDto);
            _mockBlobService.Setup(b => b.FindFileInStorageAsBase64(audio.BlobName)).Returns("base64string");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("audioBlob", result.Value.BlobName);
            Assert.Equal("base64string", result.Value.Base64);
        }
    }
}
