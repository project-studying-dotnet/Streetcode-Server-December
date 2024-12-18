using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text.GetById
{
    /// <summary>
    /// Unit tests for the <see cref="GetTextByIdHandler"/> class.
    /// Tests the handling of retrieving a text by its ID.
    /// </summary>
    public class GetTextByIdHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetTextByIdHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTextByIdHandlerTest"/> class.
        /// Sets up mocks for the repository wrapper, logger, and configures the AutoMapper.
        /// </summary>
        public GetTextByIdHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(TextProfile));
            });
            _mapper = configuration.CreateMapper();
            _handler = new GetTextByIdHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        /// <summary>
        /// Tests that the handler returns a successful result when a text is found by its ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test execution.</returns>
        [Fact]
        public async Task Handle_GetById_ReturnsTexts()
        {
            var texts = new List<TextEntity>
            {
                 new TextEntity
                {
                    Id = 1,
                    Title = "Text 1",
                    TextContent = "Some content",
                    StreetcodeId = 1,
                },
                 new TextEntity
                {
                    Id = 2,
                    Title = "Text 2",
                    TextContent = "More content",
                    StreetcodeId = 2,
                },
            };
            _mockRepositoryWrapper
                .Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TextEntity, bool>> predicate, Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>> include) =>
                    texts.AsQueryable().FirstOrDefault(predicate.Compile()));

            int id = 2;
            var query = new GetTextByIdQuery(id);
            var result = await _handler.Handle(query, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be("Text 2");
        }

        /// <summary>
        /// Tests that the handler returns an error when no text is found by the given ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test execution.</returns>
        [Fact]
        public async Task Handle_GetById_ReturnsError_WhenNoTextFound()
        {
            _mockRepositoryWrapper
                .Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TextEntity, bool>> predicate, Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>> include) =>
                    null);

            var id = 999;
            var query = new GetTextByIdQuery(id);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            _mockLogger.Verify(
                logger => logger.LogError(
                It.Is<GetTextByIdQuery>(q => q.Id == id),
                It.Is<string>(s => s == $"Cannot find any text with corresponding id: {id}")),
                Times.Once);
        }
    }
}
