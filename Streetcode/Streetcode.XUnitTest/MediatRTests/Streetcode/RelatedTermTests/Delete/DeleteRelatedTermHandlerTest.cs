using AutoMapper;
using FluentAssertions;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.Delete
{
    public class DeleteRelatedTermHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly DeleteRelatedTermHandler _handler;

        public DeleteRelatedTermHandlerTest()
        {
            this._mockLogger = new Mock<ILoggerService>();
            this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new RelatedTermProfile());
            });
            this._mapper = configuration.CreateMapper();
            this._handler = new DeleteRelatedTermHandler(this._mockRepositoryWrapper.Object, this._mapper, this._mockLogger.Object);
        }

        [Fact]
        public async Task WhenRelatedTermNotFound_thenReturnError()
        {
            // Arrange
            var command = new DeleteRelatedTermCommand("nonexistentWord");

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>>())).ReturnsAsync((RelatedTerm)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors[0].Message.Should().Be("Cannot find a related term: nonexistentWord");
        }

        [Fact]
        public async Task WhenDeleteFails_thenReturnError()
        {
            // Arrange
            var command = new DeleteRelatedTermCommand("existingWord");
            var relatedTerm = new RelatedTerm { Id = 1, Word = "existingWord", TermId = 1 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>>())).ReturnsAsync(relatedTerm);
            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors[0].Message.Should().Be("Failed to delete a related term");
        }

        [Fact]
        public async Task WhenDeleteSucceeds_thenReturnDeletedRelatedTerm()
        {
            // Arrange
            var command = new DeleteRelatedTermCommand("existingWord");
            var relatedTerm = new RelatedTerm { Id = 1, Word = "existingWord", TermId = 1 };
            var relatedTermDto = new RelatedTermDTO { Id = 1, Word = "existingWord", TermId = 1 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>>())).ReturnsAsync(relatedTerm);
            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(relatedTermDto);
        }
    }
}
