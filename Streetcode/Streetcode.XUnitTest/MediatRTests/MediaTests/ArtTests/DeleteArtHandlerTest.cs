using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Media.Images;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.ArtTests
{
    public class DeleteArtHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly DeleteArtHandler _handler;

        public DeleteArtHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ArtIsNull_ReturnsResultFail()
        {
            // Arrange
            var command = new DeleteArtCommand(1);
            string errorMsg = $"Cannot find a art with corresponding id: {command.Id}";

            SetupGetFirstOrDefault(null!);

            // Act
            var result = await _handler.Handle(command, default);

            // Arrange
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesFailed_ReturnsResultFail()
        {
            // Arrange
            var art = new Art { Id = 1, Description = "New Desc", Title = "New Title", ImageId = 1 };
            string errorMsg = $"Failed to delete a art";

            var command = new DeleteArtCommand(1);

            SetupGetFirstOrDefault(art);
            SetupSaveChanges(0);

            // Act
            var result = await _handler.Handle(command, default);

            // Arrange
            Assert.True(result.IsFailed);
            Assert.Single(result.Reasons, s => s.Message == errorMsg);
        }

        [Fact]
        public async Task Handle_SaveChangesSucceded_ReturnsResultOk()
        {
            // Arrange
            var art = new Art { Id = 1, Description = "New Desc", Title = "New Title", ImageId = 1 };
            var command = new DeleteArtCommand(1);

            SetupGetFirstOrDefault(art);

            SetupSaveChanges(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Arrange
            Assert.True(result.IsSuccess);
            Assert.Equal(Unit.Value, result.Value);
        }

        private void SetupGetFirstOrDefault(Art art)
        {
            _mockRepositoryWrapper.Setup(s => s.ArtRepository.GetFirstOrDefaultAsync(
                            It.IsAny<Expression<Func<Art, bool>>>(),
                            It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()).Result).Returns(art);
        }

        private void SetupSaveChanges(int returnVal)
        {
            _mockRepositoryWrapper.Setup(s => s.SaveChangesAsync().Result).Returns(returnVal);
        }
    }
}
