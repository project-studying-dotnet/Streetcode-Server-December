using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class CreateAudioHandlerTests
    {
        private readonly Mock<IBlobService> _mockBlob;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateAudioHandler _createAudioHandler;

        public CreateAudioHandlerTests()
        {
            _mockBlob = new Mock<IBlobService>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _createAudioHandler = new CreateAudioHandler(_mockBlob.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAudioIsCreatedSuccessfully()
        {
            //A(Arrange):

            var audioCreateDTO = new AudioFileBaseCreateDTO
            {
                Description = "description",
                Title = "testCreation",
                BaseFormat = "mp123",
                MimeType = "mpeg",
                Extension = "unknown"
            };

            var audio = new Audio
            {
                Id = 1,
                Title = audioCreateDTO.Title,
                BlobName = "blobname12345",
                MimeType = audioCreateDTO.MimeType,
                Base64 = "base64"
            };
           
            var audioDTO = new AudioDTO
            {
                Id = audio.Id,
                Description = audio.
                BlobName = audio.BlobName,
                Base64 = audio.Base64,
                MimeType = audio.MimeType
            };

            _mockMapper.Setup(m => m.Map<Audio>(audioCreateDTO)).Returns(audio);
            _mockMapper.Setup(m => m.Map<AudioDTO>(audio)).Returns(audioDTO);

            _mockRepositoryWrapper.Setup(r => r.AudioRepository.CreateAsync(audio));
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1); 

            //A(Act):

            var result = await _createAudioHandler.Handle(new CreateAudioCommand(audioCreateDTO), CancellationToken.None);

            //A(Assert):

            Assert.True(result.IsSuccess);
            Assert.Equal(audioDTO, result.Value);
            _mockBlob.Verify(b => b.SaveFileInStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockRepositoryWrapper.Verify(r => r.AudioRepository.CreateAsync(It.IsAny<Audio>()), Times.Once);
            _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockLogger.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenAudioCreationFails()
        {
            //A(Arrange):

            var audioCreateDTO = new AudioFileBaseCreateDTO
            {
                Description = "description",
                Title = "testCreation",
                BaseFormat = "mp123",
                MimeType = "mpeg",
                Extension = "unknown"
            };

            var audio = new Audio
            {
                Id = 1,
                Title = audioCreateDTO.Title,
                BlobName = "blobname12345",
                MimeType = audioCreateDTO.MimeType,
                Base64 = "base64"
            };

            var audioDTO = new AudioDTO
            {
                Id = audio.Id,
                Description = audio.
                BlobName = audio.BlobName,
                Base64 = audio.Base64,
                MimeType = audio.MimeType
            };

            _mockMapper.Setup(m => m.Map<Audio>(audioCreateDTO)).Returns(audio);
            _mockRepositoryWrapper.Setup(r => r.AudioRepository.CreateAsync(It.IsAny<Audio>()));
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            //A(Act):

            var result = await _createAudioHandler.Handle(new CreateAudioCommand(audioCreateDTO), CancellationToken.None);

            //A(Assert):

            Assert.True(result.IsFailed);
            Assert.Single(result.Errors);            
            Assert.Equal("Failed to create an audio", result.Errors[0].Message);
            _mockLogger.Verify(l => l.LogError(new CreateAudioCommand(audioCreateDTO), "Failed to create an audio"), Times.Once);
        }
    }
}
