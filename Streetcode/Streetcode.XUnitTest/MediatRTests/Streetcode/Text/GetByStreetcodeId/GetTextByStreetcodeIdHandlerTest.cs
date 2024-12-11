using AutoMapper;
using Moq;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Text;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using FluentAssertions;
using System.Linq;


namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text.GetByStreetcodeId
{
    /// <summary>
    /// Unit tests for the <see cref="GetTextByStreetcodeIdHandler"/> class.
    /// Tests the handling of retrieving a text by its ID.
    /// </summary>
    public class GetTextByStreetcodeIdHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ITextService> _mockTextService;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetTextByStreetcodeIdHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTextByStreetcodeIdHandlerTest"/> class.
        /// </summary>
        public GetTextByStreetcodeIdHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockTextService = new Mock<ITextService>();
            _mockLogger = new Mock<ILoggerService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(TextProfile));
            });

            _mapper = configuration.CreateMapper();
            _handler = new GetTextByStreetcodeIdHandler(_mockRepositoryWrapper.Object, _mapper, _mockTextService.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Add some test data to List of Text
        /// </summary>
        private List<TextEntity> GetSampleTextEntities()
        {
            return new List<TextEntity>
            {
                new TextEntity
                {
                    Id = 1,
                    Title = "Text 1",
                    TextContent = "first_text",
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
        }

        /// <summary>
        /// Function to setup the TextRepository mock with the given list of TextEntities or null if not provided.
        /// </summary>
        private void SetupTextRepository(List<TextEntity> texts)
        {
            if (texts is not null)
            {
                _mockRepositoryWrapper
                .Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TextEntity, bool>> predicate, Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>> include) =>
                    texts.AsQueryable().FirstOrDefault(predicate.Compile()));
            }
            else
            {
                _mockRepositoryWrapper
                .Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TextEntity, bool>> predicate, Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>> include) =>
                    null);
            }
        }

        /// <summary>
        /// Tests the handler when the text is found for the given streetcode id.
        /// </summary>
        [Fact]
        public async Task Handle_GetTextByStreetcodeId_Success_WhenTextFound_AndProcessedCorrectly()
        {
            int streetcodeId = 1;

            var texts = GetSampleTextEntities();
            SetupTextRepository(texts);

            _mockTextService.Setup(service => service.AddTermsTag(It.IsAny<string>())).ReturnsAsync("tagged_");

            var query = new GetTextByStreetcodeIdQuery(streetcodeId);
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be("Text 1");
            result.Value.TextContent.Should().Be("tagged_");
            _mockTextService.Verify(service => service.AddTermsTag("first_text"), Times.Once);
        }

        /// <summary>
        /// Tests the handler when no text is found and the streetcode does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_ReturnsError_WhenStreetcodeDoesNotExist()
        {
            var streetcodeId = 999;
            List<StreetcodeContent> streetcodeEntity = null;

            // mock TextRepository to return null
            SetupTextRepository(null);

            // mock StreetcodeRepository to return some streetcode even if the text is not found
            _mockRepositoryWrapper
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync((Expression<Func<StreetcodeContent, bool>> predicate, Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>> include) =>
                  null);

            var query = new GetTextByStreetcodeIdQuery(streetcodeId);
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            _mockLogger.Verify(
                logger => logger.LogError(
                       It.IsAny<GetTextByStreetcodeIdQuery>(),
                       It.Is<string>(s => s == $"Cannot find a transaction link by a streetcode id: {streetcodeId}, because such streetcode doesn`t exist")),
                Times.Once);
        }
    }
}
