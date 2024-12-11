using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Toponyms;
using Streetcode.BLL.MediatR.Toponyms.GetAll;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Toponyms.GetAll
{
    /// <summary>
    /// Unit test class for <see cref="GetAllToponymsHandler"/>.
    /// Validates the behavior of the handler when retrieving Toponym entities.
    /// </summary>
    public class GetAllToponymsHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllToponymsHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllToponymsHandlerTest"/> class.
        /// Sets up mocked dependencies and configures the AutoMapper instance.
        /// </summary>
        public GetAllToponymsHandlerTest()
        {
            _mockLogger = new Mock<ILoggerService>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(ToponymProfile));
            });
            _mapper = mapperConfiguration.CreateMapper();
            _handler = new GetAllToponymsHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        /// <summary>
        /// Configures the repository with sample Toponym data.
        /// </summary>
        private void ConfigureRepository()
        {
            var toponyms = new List<Toponym>()
            {
                new Toponym()
                {
                    Id = 1,
                    Community = "Riverdale Community",
                    AdminRegionNew = "Central District",
                    AdminRegionOld = "Old Central District",
                    Oblast = "Springfield Region",
                    StreetName = "Heroes Avenue",
                },
                new Toponym()
                {
                    Id = 2,
                    Community = "Greenwood Community",
                    AdminRegionNew = "Westside District",
                    AdminRegionOld = "Old Westside District",
                    Oblast = "Shelby Region",
                    StreetName = "Oakwood Street",
                },
                new Toponym()
                {
                    Id = 3,
                    Community = "Lakeside Community",
                    AdminRegionNew = "Northern District",
                    AdminRegionOld = "Old Northern District",
                    Oblast = "Evergreen Region",
                    StreetName = "Lakeview Drive",
                },
            };
            _mockRepositoryWrapper.Setup(repo => repo.ToponymRepository.FindAll(It.IsAny<Expression<Func<Toponym, bool>>>())).Returns(toponyms.AsQueryable());
        }

        /// <summary>
        /// Validates that the handler returns all toponyms when no title filter is provided.
        /// </summary>
        [Fact]
        public async Task Handle_WithoutTitle_ReturnsAllToponyms()
        {
            ConfigureRepository();

            var query = new GetAllToponymsQuery(
                new GetAllToponymsRequestDTO()
                {
                    Title = null,
                    Amount = 10,
                    Page = 1,
                });
            var result = await _handler.Handle(query, default);
            result.Value.Should().NotBeNull();
            result.Value.Toponyms.First().StreetName.Should().Be("Heroes Avenue");
            result.Value.Toponyms.Last().StreetName.Should().Be("Lakeview Drive");
        }

        /// <summary>
        /// Validates that the handler returns filtered toponyms based on the title query parameter.
        /// </summary>
        [Fact]
        public async Task Handle_WithTitle_ReturnsAllToponyms()
        {
            ConfigureRepository();
            var query = new GetAllToponymsQuery(
                new GetAllToponymsRequestDTO()
                {
                    Title = "ak",
                    Amount = 10,
                    Page = 1,
                });
            var result = await _handler.Handle(query, default);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().HaveCount(2);
        }

        /// <summary>
        /// Validates that the handler returns an empty result when no toponyms match the query.
        /// </summary>
        [Fact]
        public async Task Handle_WithoutToponyms_ReturnsEmptyResult()
        {
            ConfigureRepository();
            var query = new GetAllToponymsQuery(
                new GetAllToponymsRequestDTO()
                {
                    Title = "null",
                    Amount = 10,
                    Page = 1,
                });
            var result = await _handler.Handle(query, default);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().BeEmpty();
        }

        /// <summary>
        /// Validates that the handler returns the correct page of toponyms when pagination is applied.
        /// </summary>
        [Fact]
        public async Task Handle_WithPagination_ReturnCorrectPage()
        {
            ConfigureRepository();
            var query = new GetAllToponymsQuery(
                new GetAllToponymsRequestDTO()
                {
                    Title = null,
                    Amount = 2,  // amount of toponyms that can be displayed on one page
                    Page = 2, // number of page that should be returned
                });
            var result = await _handler.Handle(query, default);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().HaveCount(1); // Only one toponym should be on the second page
            result.Value.Toponyms.First().StreetName.Should().Be("Lakeview Drive");
        }
    }
}
