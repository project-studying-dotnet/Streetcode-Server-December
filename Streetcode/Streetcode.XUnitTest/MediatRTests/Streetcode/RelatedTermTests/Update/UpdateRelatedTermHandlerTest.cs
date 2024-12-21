using AutoMapper;
using FluentAssertions;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
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
using Streetcode.DAL.Specification;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.Update
{
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

            this._mockRepositoryWrapper
                .Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(existingEntity);

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

            this._mockRepositoryWrapper
                .Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>())).ReturnsAsync((RelatedTerm)null);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Cannot find any related term");
        }

        [Fact]
        public async Task WhenSaveChangesFails_ThenReturnsFailResult()
        {
            // Arrange
            var command = new UpdateRelatedTermCommand(new RelatedTermDTO { Id = 1, Word = "UpdatedWord", TermId = 2 });
            var existingEntity = new RelatedTerm { Id = 1, Word = "OldWord", TermId = 1 };

            this._mockRepositoryWrapper
                .Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(existingEntity);

            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Failed to update a related term");
        }
    }
}
