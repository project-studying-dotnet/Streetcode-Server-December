using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.ArtTests
{
    public class CreateArtHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateArtHandler _handler;

        public CreateArtHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new CreateArtHandler(
                _mapperMock.Object,
                _repositoryWrapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenArtMappingIsNull()
        {
            // Arrange
            var command = GetCreateArtCommand();

            _mapperMock.Setup(m => m.Map<Art>(It.IsAny<ArtCreateDto>())).Returns((Art)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(command, "Cannot convert null to art"), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenImageIsNotUnique()
        {
            // Arrange
            var command = GetCreateArtCommand();
            var existingArtEntity = GetArtEntity();

            _mapperMock.Setup(m => m.Map<Art>(It.IsAny<ArtCreateDto>())).Returns(existingArtEntity);

            _repositoryWrapperMock
                .Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), null))
                .ReturnsAsync(new List<Art> { existingArtEntity });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(l => l.LogError(command,
                It.Is<string>(s => s.Contains("The Art with this image already exists"))), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenStreetcodeIdsDoNotExist()
        {
            // Arrange
            var command = GetCreateArtCommand();
            var newArt = GetArtEntity();

            _mapperMock.Setup(m => m.Map<Art>(It.IsAny<ArtCreateDto>())).Returns(newArt);

            _repositoryWrapperMock
                .Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), null))
                .ReturnsAsync(new List<Art>());

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync(new List<StreetcodeContent>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(log => log.LogError(command, It.Is<string>(msg => msg.Contains("One or more Streetcode IDs do not exist"))), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenValidInput()
        {
            // Arrange
            var command = GetCreateArtCommand();

            var newArt = GetArtEntity();

            var newArtDTO = new ArtDto { Id = 1, Title = "Test Art", ImageId = 1 };

            _mapperMock.Setup(m => m.Map<Art>(It.IsAny<ArtCreateDto>())).Returns(newArt);
            _mapperMock.Setup(m => m.Map<ArtDto>(It.IsAny<Art>())).Returns(newArtDTO);

            _repositoryWrapperMock
                .Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), null))
                .ReturnsAsync(new List<Art>());

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository.GetAllAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync(new List<StreetcodeContent>
                {
                    new StreetcodeContent { Id = 1 },
                    new StreetcodeContent { Id = 2 }
                });

            _repositoryWrapperMock
                .Setup(r => r.ArtRepository.CreateAsync(It.IsAny<Art>()))
                .ReturnsAsync(newArt);

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeArtRepository.CreateRangeAsync(It.IsAny<List<StreetcodeArt>>()));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(newArt.Id, result.Value.Id);
            Assert.Equal(newArtDTO.Title, result.Value.Title);

            _repositoryWrapperMock.Verify(r => r.ArtRepository.CreateAsync(It.IsAny<Art>()), Times.Once);
            _repositoryWrapperMock.Verify(r => r.StreetcodeArtRepository
                .CreateRangeAsync(It.IsAny<List<StreetcodeArt>>()), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2)); // For Art and StreetcodeArt
        }

        [Fact]
        public async Task ShouldReturnFail_WhenArtSaveFails()
        {
            // Arrange
            var command = GetCreateArtCommand();
            var newArt = GetArtEntity();

            _mapperMock.Setup(m => m.Map<Art>(It.IsAny<ArtCreateDto>())).Returns(newArt);
            _repositoryWrapperMock
                .Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), null))
                .ReturnsAsync(new List<Art>());

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository.GetAllAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync(new List<StreetcodeContent>
                {
                    new StreetcodeContent { Id = 1 },
                    new StreetcodeContent { Id = 2 }
                });

            _repositoryWrapperMock.Setup(r => r.ArtRepository.CreateAsync(newArt)).ReturnsAsync(newArt);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(l => l.LogError(command, "Failed to create a art"), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFail_WhenStreetcodeArtSaveFails()
        {
            // Arrange
            var command = GetCreateArtCommand();

            var newArt = GetArtEntity();

            _mapperMock.Setup(m => m.Map<Art>(It.IsAny<ArtCreateDto>())).Returns(newArt);

            _repositoryWrapperMock
                .Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), null))
                .ReturnsAsync(new List<Art>());

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository.GetAllAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync(new List<StreetcodeContent>
                {
                    new StreetcodeContent { Id = 1 },
                    new StreetcodeContent { Id = 2 }
                });

            _repositoryWrapperMock.Setup(r => r.ArtRepository.CreateAsync(newArt)).ReturnsAsync(newArt);

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeArtRepository.CreateRangeAsync(It.IsAny<List<StreetcodeArt>>()));

            // Simulate successful save for Art but failure for StreetcodeArt
            _repositoryWrapperMock.SetupSequence(r => r.SaveChangesAsync())
                .ReturnsAsync(1) // Successful save for Art
                .ReturnsAsync(0); // Failed save for StreetcodeArt

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeArtRepository.CreateRangeAsync(It.IsAny<List<StreetcodeArt>>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            _loggerMock.Verify(l => l.LogError(command, "Failed to create a StreetcodeArt records"), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2)); // One for Art and one for StreetcodeArt
        }


        private static CreateArtCommand GetCreateArtCommand()
        {
            return new CreateArtCommand(new ArtCreateDto
            {
                Title = "Test Art",
                Description = "Description",
                ImageId = 1,
                StreetcodeIds = new List<int> { 1, 2 },
            });
        }

        private static Art GetArtEntity() => new Art { Id = 1, Title = "Test Art", ImageId = 1 };
    }
}
