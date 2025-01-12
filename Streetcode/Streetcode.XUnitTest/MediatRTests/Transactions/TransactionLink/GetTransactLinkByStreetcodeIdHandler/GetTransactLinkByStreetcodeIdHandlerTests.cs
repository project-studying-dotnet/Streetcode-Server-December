using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Transactions;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Streetcode;
using System.Linq.Expressions;
using Xunit;
using TransactLinkEntity = Streetcode.Domain.Entities.Transactions.TransactionLink;
namespace Streetcode.XUnitTest.MediatRTests.Transactions.TransactionLink.GetTransactLinkByStreetcodeIdHandlerTests
{
    public class GetTransactLinkByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetTransactLinkByStreetcodeIdHandler _handler;

        public GetTransactLinkByStreetcodeIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(typeof(TransactionLinkProfile)));
            _mapper = cfg.CreateMapper();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetTransactLinkByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedTransactLink_WhenTransactLinkExists()
        {
            // Arrange
            int streetcodeId = 1;
            var request = new GetTransactLinkByStreetcodeIdQuery(streetcodeId);
            var transactLinks = GetTransactLinks();

            ConfigureTransactRepository(transactLinks);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Url.Should().Be("URL");
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenTransactLinkAndStreetcodeDoNotExist()
        {
            int streetcodeId = 155;
            var transactLinks = GetTransactLinks();
            var streetcodes = GetStreetcodes();
            ConfigureStreetcodeRepository(streetcodes);
            ConfigureTransactRepository(transactLinks);
            SetupLogger();

            // Arrange
            var request = new GetTransactLinkByStreetcodeIdQuery(streetcodeId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();

            _loggerMock.Verify(logger => logger.LogError(request, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNullResult_WhenTransactLinkIsNullButStreetcodeExists()
        {
            // Arrange
            int streetcodeId = 1;
            var request = new GetTransactLinkByStreetcodeIdQuery(streetcodeId);
            var streetcode = new StreetcodeContent { Id = 1 };
            var streetcodeList = GetStreetcodes();
            ConfigureStreetcodeRepository(streetcodeList);
            ConfigureTransactRepository(null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();
        }

        private void ConfigureTransactRepository(List<TransactLinkEntity>? transactLinks)
        {
            this._repositoryWrapperMock
                .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TransactLinkEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TransactLinkEntity>, IIncludableQueryable<TransactLinkEntity, object>>>()))
                .ReturnsAsync((Expression<Func<TransactLinkEntity, bool>> predicate,
                               Func<IQueryable<TransactLinkEntity>, IIncludableQueryable<TransactLinkEntity, object>> include) =>
                {
                    return transactLinks?.AsQueryable().FirstOrDefault(predicate.Compile());
                });
        }

        private void ConfigureStreetcodeRepository(List<StreetcodeContent> streetcodes)
        {
            this._repositoryWrapperMock
            .Setup(x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync((Expression<Func<StreetcodeContent, bool>> predicate,
                Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>> include) =>
            {
                return streetcodes.AsQueryable().FirstOrDefault(predicate.Compile());
            });
        }

        private void SetupLogger()
        {
            _loggerMock.Setup(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()));
        }

        private List<TransactLinkEntity> GetTransactLinks() =>
             new()
             {
           new TransactLinkEntity { Id = 1, Url = "URL", UrlTitle = "Title", StreetcodeId = 1 },
           new TransactLinkEntity { Id = 2, Url = "URL2", UrlTitle = "Title2", StreetcodeId = 2 },
             };

        private List<StreetcodeContent> GetStreetcodes() =>
            new()
            {
            new StreetcodeContent { Id = 1 },
            new StreetcodeContent { Id = 2 },
            };
    }
}