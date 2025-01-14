using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Toponyms;
using Streetcode.BLL.MediatR.Toponyms.GetById;
using Streetcode.Domain.Entities.Toponyms;
using Streetcode.BLL.Repositories.Interfaces.Base;
namespace Streetcode.XUnitTest.MediatRTests.Toponyms.GetById
{
    /// <summary>
    /// Unit tests for the GetToponymByIdHandler class.
    /// </summary>
    public class GetToponymByIdHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetToponymByIdHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetToponymByIdHandlerTest"/> class.
        /// Sets up mocks and the handler instance for testing.
        /// </summary>
        public GetToponymByIdHandlerTest()
        {
          _mockLogger = new Mock<ILoggerService>();
          _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
          var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ToponymProfile());
            });
          _mapper = configuration.CreateMapper();
          _handler = new GetToponymByIdHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        /// <summary>
        /// Tests that the handler returns a successful result when a toponym exists.
        /// </summary>
        [Fact]
        public async Task Handle_ResultOK_WhenToponymExists()
        {
            ConfigureRepository();
            int toponimId = 1;
            var result = await this._handler.Handle(new GetToponymByIdQuery(toponimId), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(toponimId);
            result.Value.StreetName.Should().Be("Heroes Avenue");
        }

        /// <summary>
        /// Tests that the handler logs an error and returns a failed result when the repository throws an exception.
        /// Uncomment this code when try/catch statement is added to the handler.
        /// </summary>
     /*   [Fact]
        public async Task Handle_ShouldLogError_WhenRepositoryThrowsException()
        {
            int toponimId = 1;
            string errorMsg = "Database connection error";
            _mockRepositoryWrapper.Setup(x => x.ToponymRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Toponym, bool>>>(), It.IsAny<List<string>>()))
                .ThrowsAsync(new Exception(errorMsg));
            var result = await _handler.Handle(new GetToponymByIdQuery(toponimId), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().Message.Should().Contain(errorMsg);
        }*/

        /// <summary>
        /// Tests that the handler logs an error and returns a failed result when a toponym does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_ResultFail_WhenToponymDoesNotExist()
        {
            ConfigureRepository();
            int toponimId = 99;
            var result = await this._handler.Handle(new GetToponymByIdQuery(toponimId), CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            this._mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), $"Cannot find a toponym with corresponding id: {toponimId}"), Times.Once);
        }

        /// <summary>
        /// Configures the repository mock with sample data for testing.
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
            this._mockRepositoryWrapper.Setup(x => x.ToponymRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Toponym, bool>>>(), It.IsAny<List<string>>()))
                .ReturnsAsync((Expression<Func<Toponym, bool>> predicate, List<string> include) =>
                {
                    return toponyms.FirstOrDefault(predicate.Compile());
                });
        }
    }
}
