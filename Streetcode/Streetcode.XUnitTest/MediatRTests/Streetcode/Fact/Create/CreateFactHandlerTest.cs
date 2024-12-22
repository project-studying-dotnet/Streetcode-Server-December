using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.Services.Image;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.Create
{
    public class CreateFactHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly IImageService _image;
        private readonly Mock<IBlobService> _mockBlob;
        private readonly CreateFactHandler _createFactHandler;
        private readonly CreateFactDTO _createFactWithImageDto = new CreateFactDTO()
        {
            Title = "New Title",
            FactContent = "New Content",
            Image = new CreateFactImageDTO()
            {
                Title = "Image",
                ImageDetails = new CreateFactImageDetailsDTO()
                {
                    Title = "Image Description"
                }
            }
        };

        public CreateFactHandlerTest()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new FactProfile())).CreateMapper();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockBlob = new Mock<IBlobService>();
            _image = new ImageService(_mapper, _mockBlob.Object);
            _createFactHandler = new CreateFactHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object, _image);
        }

        [Fact]
        public async Task Handle_FactIsNull_ReturnsResultFail()
        {
            // Arrange
            var command = new CreateFactCommand(null!);
            const string errorMsg = "Cannot create new fact!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_FactImageIsNull_ReturnsResultFail()
        {
            // Arrange
            var _createFactDto = new CreateFactDTO() { Title = "New Title", FactContent = "New Content" };
            SetupGetAll();
            var command = new CreateFactCommand(_createFactDto);
            const string errorMsg = "Cannot create an image!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_CreatedFactIsNull_ReturnsResultFail()
        {
            // Arrange
            SetupGetAll();
            SetupSaveFileInStorage();
            SetupCreate(null!);

            var command = new CreateFactCommand(_createFactWithImageDto);
            const string errorMsg = "Cannot create fact!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesResultIsNotSuccess_ReturnsResultFail()
        {
            // Arrange
            var fact = _mapper.Map<FactEntity>(_createFactWithImageDto);

            SetupGetAll();
            SetupSaveFileInStorage();
            SetupCreate(fact);
            SetupSaveChanges(0);

            var command = new CreateFactCommand(_createFactWithImageDto);
            const string errorMsg = "Cannot save changes in the database!";

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_FactCreatedSuccessfully_ReturnsResultOK()
        {
            // Arrange
            var fact = _mapper.Map<FactEntity>(_createFactWithImageDto);
            SetupGetAll();
            SetupSaveFileInStorage();
            SetupCreate(fact);
            SetupSaveChanges(1);

            var command = new CreateFactCommand(_createFactWithImageDto);

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_IndexIsCorrect()
        {
            // Arrange
            var fact = new FactEntity { Title = "New Title", FactContent = "New Content", ImageId = 1, Index = 4 };

            _mockRepositoryWrapper.Setup(p => p.FactRepository.CreateAsync(It.IsAny<FactEntity>()).Result).Returns<FactEntity>(r => r);
            SetupGetAll();
            SetupSaveFileInStorage();
            SetupSaveChanges(1);

            var command = new CreateFactCommand(_createFactWithImageDto);

            // Act
            var result = await _createFactHandler.Handle(command, default);

            // Assert
            Assert.Equal(4, result.Value.Index);
        }

        private void SetupGetAll()
        {
            var facts = new List<FactEntity>
            {
                new FactEntity { Id = 2, ImageId = 1, Index = 1 },
                new FactEntity { Id = 3, ImageId = 2, Index = 3 },
            };

            _mockRepositoryWrapper.Setup(p => p.FactRepository.GetAllAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), It.IsAny<Func<IQueryable<FactEntity>,
                IIncludableQueryable<FactEntity, object>>>()).Result).Returns(facts);
        }

        private void SetupCreate(FactEntity fact)
        {
            _mockRepositoryWrapper.Setup(p => p.FactRepository.CreateAsync(It.IsAny<FactEntity>()).Result).Returns(fact);
        }

        private void SetupSaveChanges(int returnValue)
        {
            _mockRepositoryWrapper.Setup(p => p.SaveChangesAsync().Result).Returns(returnValue);
        }

        private void SetupSaveFileInStorage()
        {
            _mockBlob.Setup(s => s.SaveFileInStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("HashBlobStorageName");
        }
    }
}
