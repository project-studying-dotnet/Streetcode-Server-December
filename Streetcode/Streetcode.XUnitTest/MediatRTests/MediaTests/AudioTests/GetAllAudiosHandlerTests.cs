using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetAll;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Media.Images;
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
    public class GetAllAudiosHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBlobService> _mockBlob;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllAudiosHandler _getAllAudiosHandler;

        public GetAllAudiosHandlerTests()
        {
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockBlob = new Mock<IBlobService>();
            _mockLogger = new Mock<ILoggerService>();
            _getAllAudiosHandler = new GetAllAudiosHandler(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockBlob.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllAudioDTOs_WhenAudiosExist()
        {
            // Arrange
            var audios = new List<Audio>
            {
                new Audio { Id = 1, Title = "Audio1", BlobName = "blob1" },
                new Audio { Id = 2, Title = "Audio2", BlobName = "blob2" },
                new Audio { Id = 3, Title = "Audio3", BlobName = "blob3" }
            };

            var audioDTOs = new List<AudioDto>
            {
                new AudioDto { Id = 1, Description = "Audio1", BlobName = "blob1" },
                new AudioDto { Id = 2, Description = "Audio2", BlobName = "blob2" },
                new AudioDto { Id = 3, Description = "Audio3", BlobName = "blob3" }
            };

            _mockRepository.Setup(r => r.AudioRepository.GetAllAsync(It.IsAny<Expression<Func<Audio, bool>>>(), It.IsAny<Func<IQueryable<Audio>, IIncludableQueryable<Audio, object>>>())).ReturnsAsync(audios);
            _mockMapper.Setup(m => m.Map<IEnumerable<AudioDto>>(audios)).Returns(audioDTOs);
            _mockBlob.Setup(b => b.FindFileInStorageAsBase64(It.IsAny<string>())).Returns((string blobName) => Task.FromResult(Convert.ToBase64String(Encoding.UTF8.GetBytes(blobName))));

            // Act
            var result = await _getAllAudiosHandler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(audioDTOs.Count, result.Value.Count());
            Assert.Collection(result.Value,
            audios.Select(expected => (Action<AudioDto>)(actual =>
            {
                    Assert.Equal(expected.Id, actual.Id);
                    Assert.Equal(expected.Title, actual.Description);
                    Assert.Equal(expected.BlobName, actual.BlobName);
                })).ToArray()
            );

            _mockRepository.Verify(r => r.AudioRepository.GetAllAsync(It.IsAny<Expression<Func<Audio, bool>>>(), It.IsAny<Func<IQueryable<Audio>, IIncludableQueryable<Audio, object>>>()), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<AudioDto>>(audios), Times.Once);
            _mockBlob.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Exactly(audioDTOs.Count));
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNoAudiosExist()
        {
            // Arrange

            _mockRepository.Setup(r => r.AudioRepository.GetAllAsync(It.IsAny<Expression<Func<Audio, bool>>>(), It.IsAny<Func<IQueryable<Audio>, IIncludableQueryable<Audio, object>>>())).ReturnsAsync((IEnumerable<Audio>)null);

            // Act

            var result = await _getAllAudiosHandler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Errors);
            Assert.Equal("Cannot find any audio", result.Errors[0].Message);

            _mockLogger.Verify(l => l.LogError(new GetAllAudiosQuery(), "Cannot find any audio"), Times.Once);
            _mockRepository.Verify(r => r.AudioRepository.GetAllAsync(It.IsAny<Expression<Func<Audio, bool>>>(), It.IsAny<Func<IQueryable<Audio>, IIncludableQueryable<Audio, object>>>()), Times.Once);
        }
    }
}
