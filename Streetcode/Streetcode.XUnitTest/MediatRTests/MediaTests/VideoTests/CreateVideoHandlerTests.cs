using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.VideoTests
{
    public class CreateVideoHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateVideoHandler _createVideoHandler;

        public CreateVideoHandlerTests()
        {       
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _createVideoHandler = new CreateVideoHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenVideoIsCreatedSuccessfully()
        {
            // A(Arrange):

            var videoCreateDTO = new VideoCreateDTO
            {
                Url = "https://example.com",
                StreetcodeId = 1
            };

            var video = new Video
            {
                Id = 1,
                Title = "Title",
                Description = "Description",
                Url = "https://example.com",
                StreetcodeId = 1
            };

            var videoDTO = new VideoDTO
            {
                Id = 1,
                Description = "Description",
                Url = "https://example.com",
                StreetcodeId = 1
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1
            };

            _mockMapper.Setup(m => m.Map<Video>(videoCreateDTO)).Returns(video);
            _mockMapper.Setup(m => m.Map<VideoDTO>(video)).Returns(videoDTO);

            _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), 
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>() 
            ))
            .ReturnsAsync(streetcode); 

            _mockRepositoryWrapper.Setup(r => r.VideoRepository.CreateAsync(It.IsAny<Video>()))
                .ReturnsAsync((Video v) => v);

            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // A(Act):

            var result = await _createVideoHandler.Handle(new CreateVideoCommand(videoCreateDTO), CancellationToken.None);

            // A(Assert):

            Assert.True(result.IsSuccess);
            Assert.Equal(videoDTO, result.Value);
            _mockRepositoryWrapper.Verify(r => r.VideoRepository.CreateAsync(It.IsAny<Video>()), Times.Once);
            _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockLogger.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenAudioCreationFails()
        {
            // A(Arrange):

            var videoCreateDTO = new VideoCreateDTO
            {
                Url = "https://example.com",
                StreetcodeId = 1
            };

            var video = new Video
            {
                Id = 1,
                Title = "Title",
                Description = "Description",
                Url = "https://example.com",
                StreetcodeId = 1
            };

            var streetcode = new StreetcodeContent
            {
                Id = 1
            };

            _mockMapper.Setup(m => m.Map<Video>(videoCreateDTO)).Returns(video);

            _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
              It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
              It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>()
            ))
            .ReturnsAsync(streetcode);

            _mockRepositoryWrapper.Setup(r => r.VideoRepository.CreateAsync(It.IsAny<Video>()))
                .ReturnsAsync((Video v) => v);

            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // A(Act):

            var result = await _createVideoHandler.Handle(new CreateVideoCommand(videoCreateDTO), CancellationToken.None);

            // A(Assert):

            Assert.True(result.IsFailed);
            Assert.Single(result.Errors);
            Assert.Equal("Failed to create a video", result.Errors[0].Message);
            _mockLogger.Verify(l => l.LogError(new CreateVideoCommand(videoCreateDTO), "Failed to create a video"), Times.Once);
        }
    }
}
