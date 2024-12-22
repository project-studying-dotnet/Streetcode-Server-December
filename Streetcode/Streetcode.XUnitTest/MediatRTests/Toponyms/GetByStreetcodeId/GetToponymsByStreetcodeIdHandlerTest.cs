using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Toponyms;
using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
namespace Streetcode.XUnitTest.MediatRTests.Toponyms.GetByStreetcodeId
{
    /// <summary>
    /// Test Class for the GetToponymsByStreetcodeIdHandler.
    /// </summary>
    public class GetToponymsByStreetcodeIdHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockedRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockedLogger;
        private readonly IMapper _mapper;
        private readonly GetToponymsByStreetcodeIdHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetToponymsByStreetcodeIdHandlerTest"/> class.
        /// Sets up mocks and the handler instance for testing.
        /// </summary>
        public GetToponymsByStreetcodeIdHandlerTest()
        {
            _mockedLogger = new Mock<ILoggerService>();
            _mockedRepositoryWrapper = new Mock<IRepositoryWrapper>();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(ToponymProfile));
            });
            _mapper = mapperConfiguration.CreateMapper();
            _handler = new GetToponymsByStreetcodeIdHandler(_mockedRepositoryWrapper.Object, _mapper, _mockedLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsToponyms_WhenToponymsAreFound()
        {
            ConfigureRepository();
            int streetcodeId = 1;
            var query = new GetToponymsByStreetcodeIdQuery(streetcodeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.First().StreetName.Should().Be("Taras Shevchenko Street");
            result.Value.Last().StreetName.Should().Be("Shevchenko Lane");
        }

        /// <summary>
        /// Test case: Handles the scenario when no toponyms with streetcodeId are found in the repository.
        /// </summary>
        [Fact]
        public async Task Handle_ReturnsFailure_WhenNoToponymsFound()
        {
            ConfigureRepository();
            int streetcodeId = 999;
            var query = new GetToponymsByStreetcodeIdQuery(streetcodeId);
            var result = await _handler.Handle(query, CancellationToken.None);
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(e => e.Message == $"Cannot find a toponym by a streetcode id: {streetcodeId}");
        }

        /// <summary>
        /// Test case: Handles the scenario when toponyms is null.
        /// </summary>
        [Fact]
        public async Task Handle_ReturnsFailure_WhenToponymsIsNull()
        {
            int streetcodeId = 1;
            _mockedRepositoryWrapper.Setup(r => r.ToponymRepository.GetAllAsync(
               It.IsAny<Expression<Func<Toponym, bool>>>(),
               It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
               )).ReturnsAsync((Expression<Func<Toponym, bool>> predicate,
                                Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>> _) =>
               {
                   return null;
               });
            var query = new GetToponymsByStreetcodeIdQuery(streetcodeId);
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(e => e.Message == $"Cannot find a toponym by a streetcode id: {streetcodeId}");
        }

        /// <summary>
        /// Helper method to setup the repository with a list of toponyms.
        /// </summary>
        private void ConfigureRepository()
        {
            var toponyms = new List<Toponym>()
            {
                new Toponym()
                {
                    Id = 1,
                    Community = "Kyiv Oblast",
                    AdminRegionNew = "Shevchenkivskyi District",
                    AdminRegionOld = "Soviet District",
                    Oblast = "Kyiv Oblast",
                    StreetName = "Taras Shevchenko Street",
                    Streetcodes = new List<StreetcodeContent>()
                    {
                        new StreetcodeContent()
                        {
                            Id = 1, // Streetcode for Taras Shevchenko
                            Index = 1,
                            DateString = "1814-03-09",
                            Title = "Taras Shevchenko - The Great Kobzar",
                            TransliterationUrl = "taras-shevchenko-biography",
                            Status = StreetcodeStatus.Published,
                            ViewCount = 5000,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            EventStartOrPersonBirthDate = new DateTime(1814, 3, 9),
                            EventEndOrPersonDeathDate = new DateTime(1861, 3, 10),
                            Teaser = "The life and works of the great Ukrainian poet",
                        },
                    },
                },
                new Toponym()
                {
                    Id = 2,
                    Community = "Cherkasy Oblast",
                    AdminRegionNew = "Kaniv District",
                    AdminRegionOld = "Kaniv District",
                    Oblast = "Cherkasy Oblast",
                    StreetName = "Shevchenko Lane",
                    Streetcodes = new List<StreetcodeContent>()
                    {
                        new StreetcodeContent()
                        {
                            Id = 1, // Streetcode for Taras Shevchenko
                            Index = 2,
                            DateString = "1814-03-09",
                            Title = "Taras Shevchenko - The Great Kobzar",
                            TransliterationUrl = "taras-shevchenko-biography",
                            Status = StreetcodeStatus.Published,
                            ViewCount = 5000,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            EventStartOrPersonBirthDate = new DateTime(1814, 3, 9),
                            EventEndOrPersonDeathDate = new DateTime(1861, 3, 10),
                            Teaser = "The life and works of the great Ukrainian poet",
                        },
                    },
                },
                new Toponym()
                {
                    Id = 3,
                    Community = "Lviv Oblast",
                    AdminRegionNew = "Lychakiv District",
                    AdminRegionOld = "Lychakiv District",
                    Oblast = "Lviv Oblast",
                    StreetName = "Skovorody Boulevard",
                    Streetcodes = new List<StreetcodeContent>()
                    {
                        new StreetcodeContent()
                        {
                            Id = 2, // Streetcode for another personality
                            Index = 3,
                            DateString = "2024-01-01",
                            Title = "Skovoroda",
                            TransliterationUrl = "shevchenko-memorial",
                            Status = StreetcodeStatus.Published,
                            ViewCount = 3000,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            EventStartOrPersonBirthDate = new DateTime(1961, 3, 10),
                            EventEndOrPersonDeathDate = new DateTime(2024, 1, 1),
                            Teaser = "Modern events honoring Hryhorii Skovoroda",
                        },
                    },
                },
            };
            _mockedRepositoryWrapper.Setup(r => r.ToponymRepository.GetAllAsync(
                It.IsAny<Expression<Func<Toponym, bool>>>(),
                It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                )).ReturnsAsync((Expression<Func<Toponym, bool>> predicate,
                                 Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>> _) =>
                {
                    return toponyms.AsQueryable().Where(predicate).ToList();
                });
        }
    }
}
