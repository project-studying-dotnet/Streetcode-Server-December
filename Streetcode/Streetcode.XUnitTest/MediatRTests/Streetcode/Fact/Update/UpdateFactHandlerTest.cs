using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.Update
{
    /// <summary>
    /// Unit tests for <see cref="UpdateFactHandler"/>.
    /// </summary>
    public class UpdateFactHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly IMapper _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly UpdateFactHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFactHandlerTest"/> class.
        /// </summary>
        public UpdateFactHandlerTest()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(typeof(FactProfile)));
            _mapperMock = configuration.CreateMapper();
            _handler = new UpdateFactHandler(_repositoryWrapperMock.Object, _mapperMock, _loggerMock.Object);
        }

        /// <summary>
        /// Verifies that the handler successfully updates and returns a fact when it exists.
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// </summary>
        [Fact]
        public async Task Handle_FactExists_ShouldUpdateAndReturnFact()
        {
            var updatedFactDTO = new FactDto { Id = 1, Title = "Updated Title", FactContent = "Updated Content", ImageId = 2 };
            var facts = CreateFakeData();

            _repositoryWrapperMock.Setup(x => x.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>()))
                .ReturnsAsync((Expression<Func<FactEntity, bool>> predicate, Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>> include) =>
                {
                    return facts.AsQueryable().FirstOrDefault(predicate.Compile());
                });

            _repositoryWrapperMock.Setup(x => x.FactRepository.Update(It.IsAny<FactEntity>()));
            _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var command = new UpdateFactCommand(updatedFactDTO);
            var result = await _handler.Handle(command, default);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be(updatedFactDTO.Title);
        }

        /// <summary>
        /// Verifies that the handler returns a failure when the fact does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_FactDoesNotExist_ShouldReturnFailure()
        {
            var factDto = new FactDto { Id = 1, Title = "Updated Title", FactContent = "Updated Content", ImageId = 2 };

            _repositoryWrapperMock.Setup(x => x.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>()))
                .ReturnsAsync((Expression<Func<FactEntity, bool>> predicate, Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>> include) => null);

            // Act
            var command = new UpdateFactCommand(factDto);
            var result = await _handler.Handle(command, default);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(e => e.Message == $"Fact with id {factDto.Id} not found");
            _loggerMock.Verify(l => l.LogError(command, $"Fact with id {factDto.Id} not found"));
        }

        /// <summary>
        /// Verifies that the handler returns a failure when an exception occurs during the update.
        /// </summary>
        [Fact]
        public async Task Handle_UpdateThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var updatedFactDTO = new FactDto { Id = 1, Title = "Updated Title", FactContent = "Updated Content", ImageId = 2 };
            var facts = CreateFakeData();

            _repositoryWrapperMock.Setup(x => x.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>()))
                .ReturnsAsync((Expression<Func<FactEntity, bool>> predicate, Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>> include) =>
                {
                    return facts.AsQueryable().FirstOrDefault(predicate.Compile());
                });

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database error"));

            var command = new UpdateFactCommand(updatedFactDTO);
            var result = await _handler.Handle(command, default);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(e => e.Message == "An error occurred while updating the fact.");

            _loggerMock.Verify(
                l => l.LogError(
                It.Is<Exception>(ex => ex.Message == "Database error"),
                "An error occurred while updating the fact."),
                Times.Once);

            _repositoryWrapperMock.Verify(r => r.FactRepository.Update(It.IsAny<FactEntity>()), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Creates fake data for testing purposes.
        /// </summary>
        /// <returns>A list of fake <see cref="FactEntity"/> objects.</returns>
        private List<FactEntity> CreateFakeData()
        {
            return new List<FactEntity>
            {
                new FactEntity { Id = 1, Title = "First Title", FactContent = "First fact content", ImageId = 1 },
                new FactEntity { Id = 2, Title = "Second Title", FactContent = "Second fact content", ImageId = 1 },
                new FactEntity { Id = 3, Title = "Third Title", FactContent = "Third fact content", ImageId = 1 },
            };
        }
    }
}