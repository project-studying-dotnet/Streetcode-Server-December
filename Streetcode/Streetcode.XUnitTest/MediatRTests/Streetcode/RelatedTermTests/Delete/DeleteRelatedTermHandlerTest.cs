using FluentAssertions;
using FluentResults;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Moq;
using Xunit;
using Streetcode.DAL.Specification;
using MediatR;
using Streetcode.Domain.Entities.Streetcode.TextContent;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.Delete
{
    public class DeleteRelatedTermHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly DeleteRelatedTermHandler _handler;

        public DeleteRelatedTermHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _handler = new DeleteRelatedTermHandler(_mockRepositoryWrapper.Object);
        }

        [Fact]
        public async Task ShouldThrowException_WhenRelatedTermNotFound()
        {
            // Arrange
            var command = new DeleteRelatedTermCommand("nonexistentWord", 1);

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<RelatedTerm>>())).ReturnsAsync((RelatedTerm)null);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            exception.Message.Should().Be("Cannot find any word");
        }

        [Fact]
        public async Task ShouldThrowException_WhenDeleteFails()
        {
            // Arrange
            var command = new DeleteRelatedTermCommand("existingWord", 1);
            var relatedTerm = new RelatedTerm { Id = 1, Word = "existingWord", TermId = 1 };

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<RelatedTerm>>())).ReturnsAsync(relatedTerm);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            exception.Message.Should().Be("Failed to delete a word");
        }

        [Fact]
        public async Task WhenDeleteSucceeds_ThenReturnDeletedRelatedTerm()
        {
            // Arrange
            var command = new DeleteRelatedTermCommand("existingWord", 1);
            var relatedTerm = new RelatedTerm { Id = 1, Word = "existingWord", TermId = 1 };

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(
                It.IsAny<IBaseSpecification<RelatedTerm>>())).ReturnsAsync(relatedTerm);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(Unit.Value);
        }
    }
}
