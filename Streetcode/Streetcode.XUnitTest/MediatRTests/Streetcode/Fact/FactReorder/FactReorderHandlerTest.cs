using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Specifications;
using Xunit;
using FactEntity = Streetcode.Domain.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.FactReorder
{
    public class FactReorderHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly FactReorderHandler _handler;

        public FactReorderHandlerTest()
        {
            _mockLogger = new Mock<ILoggerService>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FactProfile());
            });
            _mapper = configuration.CreateMapper();
            _handler = new FactReorderHandler(_mapper, _mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenFactsAreReorderedSuccessfully_ReturnsOkResult()
        {
            // Arrange
            var idPositions = new List<int> { 2, 1 };

            var factReorderDto = new FactReorderDto
            {
                StreetcodeId = 1,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<FactEntity>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                    Index = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                   Index = 2,
                },
            };

            _mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<FactEntity>>()))
                .ReturnsAsync(existingFacts.Where(x => x.StreetcodeId == 1));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal(2, result.Value[0].Index);
            Assert.Equal(1, result.Value[1].Index);
        }

        [Fact]
        public async Task Handle_FactsNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var idPositions = new List<int> { 2, 1 };

            var factReorderDto = new FactReorderDto
            {
                StreetcodeId = 1,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<FactEntity>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                    Index = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                   Index = 2,
                },
            };

            _mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<FactEntity>>()))
                .ReturnsAsync((List<FactEntity>)null!);

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Entities not found!", result.Message);
        }

        [Fact]
        public async Task Handle_WhenIdPositionsMatch_ThrowException()
        {
            // Arrange
            var idPositions = new List<int> { 1, 1 };

            var factReorderDto = new FactReorderDto
            {
                StreetcodeId = 1,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<FactEntity>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                    Index = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                   Index = 2,
                },
            };

            _mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<FactEntity>>()))
                .ReturnsAsync(existingFacts.Where(x => x.StreetcodeId == 1));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("IdPositions should not have duplicates", result.Message);
        }

        [Fact]
        public async Task Handle_WhenGivingWrongIdPositions_ThrowsException()
        {
            // Arrange
            var idPositions = new List<int> { 1, 2 };

            var factReorderDto = new FactReorderDto
            {
                StreetcodeId = 1,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<FactEntity>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                    Index = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                   Index = 2,
                },
                new ()
                {
                   Id = 3,
                   StreetcodeId = 1,
                   Index = 3,
                },
            };

            _mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<FactEntity>>()))
                .ReturnsAsync(existingFacts.Where(x => x.StreetcodeId == 1));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("All IdPositions should be related to Facts with provided StreetcodeId", result.Message);
        }

        [Fact]
        public async Task Handle_WhenFailedToSave_ThrowException()
        {
            // Arrange
            var idPositions = new List<int> { 1, 2 };

            var factReorderDto = new FactReorderDto
            {
                StreetcodeId = 1,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<FactEntity>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                    Index = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                   Index = 2,
                },
                new ()
                {
                   Id = 3,
                   StreetcodeId = 2,
                   Index = 3,
                },
            };

            _mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<FactEntity>>()))
                .ReturnsAsync(existingFacts.Where(x => x.StreetcodeId == 1));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Failed to save the reordered facts.", result.Message);
        }
    }
}
