using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Streetcode;
using Streetcode.BLL.Mapping.Streetcode.Types;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.Audio;
using Streetcode.BLL.Services.Image;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Streetcode.CreateMainPage
{
    public class CreateStreetcodeMainPageHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly IImageService _image;
        private readonly IAudioService _audio;
        private readonly Mock<IBlobService> _mockBlob;
        private readonly CreateStreetcodeMainPageHandler _handler;
        private readonly StreetcodeMainPageCreateDto _streetcodeMainPageCreateDto = new()
        {
            Title = "New Title",
            StreetcodeType = StreetcodeType.Person,
            Tags = Array.Empty<StreetcodeTagDto>(),
            TransliterationUrl = "",
            Images = new List<ImageFileBaseCreateDto>
            {
                new ImageFileBaseCreateDto
                {
                    Title = "Animation",
                },
                new ImageFileBaseCreateDto
                {
                    Title = "Picture",
                }
            },
            Audio = new AudioFileBaseCreateDto
            {
                Title = "Audio Title",
                Description = "Audio Description"
            }
        };
        private readonly StreetcodeContent _streetcodeContent = new()
        {
            Title = "New Title",
            Images = new List<Image>
            {
                new Image
                {
                    Id = 1,
                    MimeType = "gif",
                },
                new Image
                {
                    Id = 2,
                    MimeType = "jpeg",
                }
            },
            Audio = new Audio
            {
                Id = 1,
                Title = "Audio Title",
            }
        };

        public CreateStreetcodeMainPageHandlerTest()
        {
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new StreetcodeProfile());
                    cfg.AddProfile(new EventStreetcodeProfile());
                    cfg.AddProfile(new PersonStreetcodeProfile());
                    cfg.AddProfile(new ImageProfile());
                    cfg.AddProfile(new AudioProfile());
                }).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockBlob = new Mock<IBlobService>();
            _image = new ImageService(_mapper, _mockBlob.Object);
            _audio = new AudioService(_mapper, _mockBlob.Object);
            _handler = new CreateStreetcodeMainPageHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object, _image, _audio);
        }

        [Fact]
        public async Task Handle_StreetcodeMainPageIsNull_ReturnsResultFail()
        {
            var command = new CreateStreetcodeMainPageCommand(null!);
            string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "main page block");

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_StreetcodeMainPageCreatedIsNull_ReturnsResultFail()
        {
            // Arrange

            var command = new CreateStreetcodeMainPageCommand(_streetcodeMainPageCreateDto);
            string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "main page block");

            _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.CreateAsync(It.IsAny<StreetcodeContent>())).ReturnsAsync((StreetcodeContent)null!);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesResultIsNotSuccess_ReturnsResultFail()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.CreateAsync(It.IsAny<StreetcodeContent>()).Result)
                .Returns(_streetcodeContent);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync().Result).Returns(0);

            var command = new CreateStreetcodeMainPageCommand(_streetcodeMainPageCreateDto);
            string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "main page block");

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_StreetcodeMainPageCreatedSuccessfully_ReturnsResultOK()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.CreateAsync(It.IsAny<StreetcodeContent>()).Result)
                .Returns(_streetcodeContent);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync().Result).Returns(1);

            var command = new CreateStreetcodeMainPageCommand(_streetcodeMainPageCreateDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
