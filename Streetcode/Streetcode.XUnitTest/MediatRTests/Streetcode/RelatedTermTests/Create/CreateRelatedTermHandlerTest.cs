namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.Create
{
    using AutoMapper;
    using FluentAssertions;
    using global::Streetcode.BLL.DTO.Streetcode.TextContent;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode.TextContent;
    using global::Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
    using global::Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
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
    public class CreateRelatedTermHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreateRelatedTermHandler _handler;

        public CreateRelatedTermHandlerTest()
        {
            this._mockLogger = new Mock<ILoggerService>();
            this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new RelatedTermProfile());
            });
            this._mapper = configuration.CreateMapper();
            this._handler = new CreateRelatedTermHandler(this._mockRepositoryWrapper.Object, this._mapper, this._mockLogger.Object);
        }

        [Fact]
        public async Task WhenCreateRelatedTermCommandIsValid_ThenReturnsOkWithDTO()
        {
            //Arrange
            var command = new CreateRelatedTermCommand(new RelatedTermDTO { Word = "Test", TermId = 1 });
            var entity = new RelatedTerm { Id = 1, Word = "Test", TermId = 1 };
            var createdDto = new RelatedTermDTO { Id = 1, Word = "Test", TermId = 1 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetAllAsync(It.IsAny<Expression<Func<RelatedTerm, bool>>>(), null)).ReturnsAsync(new List<RelatedTerm>());
            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.Create(It.IsAny<RelatedTerm>())).Returns(entity);
            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Word.Should().Be("Test");
            result.Value.TermId.Should().Be(1);
        }

        [Fact]
        public async Task WhenRelatedTermAlreadyExists_ThenReturnsFailResult()
        {
            // Arrange
            var command = new CreateRelatedTermCommand(new RelatedTermDTO { Word = "Test", TermId = 1 });
            var existingEntity = new RelatedTerm { Id = 1, Word = "Test", TermId = 1 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetAllAsync(It.IsAny<Expression<Func<RelatedTerm, bool>>>(), null)).ReturnsAsync(new List<RelatedTerm> { existingEntity });

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Слово з цим визначенням уже існує");
        }

        [Fact]
        public async Task WhenDatabaseSaveFails_ThenReturnsFailResult()
        {
            // Arrange
            var command = new CreateRelatedTermCommand(new RelatedTermDTO { Word = "Test", TermId = 1 });
            var entity = new RelatedTerm { Word = "Test", TermId = 1 };
            var createdDto = new RelatedTermDTO { Id = 1, Word = "Test", TermId = 1 };

            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.GetAllAsync(It.IsAny<Expression<Func<RelatedTerm, bool>>>(), null)).ReturnsAsync(new List<RelatedTerm>());
            this._mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.Create(It.IsAny<RelatedTerm>())).Returns(entity);
            this._mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Cannot save changes in the database after related word creation!");
        }

        [Fact]
        public async Task WhenRelatedTermIsNull_ThenReturnsFailResult()
        {
            // Arrange
            var command = new CreateRelatedTermCommand(null);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Message == "Cannot create new related word for a term!");
        }
    }
}
