using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Entities.Streetcode;
using System.Linq.Expressions;
using FluentAssertions;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text.GetAll
{
    /// <summary>
    /// Unit test class for <see cref="GetAllTextsHandler" />.
    /// </summary>
    public class GetAllTextsHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllTextsHandler _handler;

        public GetAllTextsHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(TextProfile));
            });
            _mapper = configuration.CreateMapper();
            _handler = new GetAllTextsHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTexts()
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
                .Setup(repo => repo.TextRepository.GetAllAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>()))
                .ReturnsAsync(texts);

            var query = new GetAllTextsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Count().Should().Be(2);
            result.Value.First().Title.Should().Be("Text 1");

        }

        [Fact]
        public async Task Handle_ReturnsError_WhenNoTextFound()
        {
            List<TextEntity> text = null;
            _mockRepositoryWrapper
                .Setup(repo => repo.TextRepository.GetAllAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>()))
                .ReturnsAsync(text);
            var query = new GetAllTextsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<GetAllTextsQuery>(), It.Is<string>(s => s == "Cannot find any text")), Times.Once);
        }
    }
}
