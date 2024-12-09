namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.Update
{
    using AutoMapper;
    using FluentAssertions;
    using global::Streetcode.BLL.DTO.Streetcode.TextContent;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode.TextContent;
    using global::Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
    using global::Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
    using global::Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
    using global::Streetcode.DAL.Entities.Streetcode.TextContent;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    public class UpdateRelatedTermHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly UpdateRelatedTermHandler _handler;

        public UpdateRelatedTermHandlerTest()
        {
            this._mockLogger = new Mock<ILoggerService>();
            this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new RelatedTermProfile());
            });
            this._mapper = configuration.CreateMapper();
            this._handler = new UpdateRelatedTermHandler(this._mapper, this._mockRepositoryWrapper.Object, this._mockLogger.Object);
        }

        [Fact]
        public async Task WhenUpdateRelatedTermCommandIsValid_ThenReturnsOkWithUpdatedDTO()
        {
            // Arrange
            var command = new UpdateRelatedTermCommand(new RelatedTermDTO { Id = 1, Word = "UpdatedWord", TermId = 2 });
            var existingEntity = new RelatedTerm { Id = 1, Word = "OldWord", TermId = 1 };
            var updatedEntity = new RelatedTerm { Id = 1, Word = "UpdatedWord", TermId = 2 };
            var updatedDto = new RelatedTermDTO { Id = 1, Word = "UpdatedWord", TermId = 2 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>>())).ReturnsAsync(existingEntity);

            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);


            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Word.Should().Be("UpdatedWord");
            result.Value.TermId.Should().Be(2);
        }

        [Fact]
        public async Task WhenRelatedTermNotFound_ThenReturnsFailResult()
        {
            // Arrange
            var command = new UpdateRelatedTermCommand(new RelatedTermDTO { Id = 1, Word = "UpdatedWord", TermId = 2 });

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>>())).ReturnsAsync((RelatedTerm)null);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Related term not found!");
        }

        [Fact]
        public async Task WhenSaveChangesFails_ThenReturnsFailResult()
        {
            // Arrange
            var command = new UpdateRelatedTermCommand(new RelatedTermDTO { Id = 1, Word = "UpdatedWord", TermId = 2 });
            var existingEntity = new RelatedTerm { Id = 1, Word = "OldWord", TermId = 1 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>>())).ReturnsAsync(existingEntity);

            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Failed to save the updated related term.");
        }
    }
}
