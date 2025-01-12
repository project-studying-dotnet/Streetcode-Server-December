using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetById;
using Streetcode.BLL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using TransactLinkEntity = Streetcode.Domain.Entities.Transactions.TransactionLink;
namespace Streetcode.XUnitTest.MediatRTests.Transactions.TransactionLink.GetTransactLinkByIdHandlerTests
{
    public class GetTransactLinkByIdHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetTransactLinkByIdHandler _handler;

        public GetTransactLinkByIdHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactLinkEntity, TransactLinkDto>();
            });
            _mapper = mapperConfig.CreateMapper();

            _handler = new GetTransactLinkByIdHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenEntityExists()
        {
            List<TransactLinkEntity> transactLinks = GetTransactLinks();
            ConfigureRepositoryToReturn(transactLinks);
            int transactionId = 1;
            var query = new GetTransactLinkByIdQuery(transactionId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(1);
            result.Value.Url.Should().Be("URL");
            result.Value.StreetcodeId.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenEntityDoesNotExist()
        {
            int transactionId = 1;
            ConfigereRepositoryToReturnNull();
            var query = new GetTransactLinkByIdQuery(transactionId);
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        private void ConfigureRepositoryToReturn(List<TransactLinkEntity> transactLinks)
        {
            this._mockRepositoryWrapper
            .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactLinkEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TransactLinkEntity>, IIncludableQueryable<TransactLinkEntity, object>>>()))
            .ReturnsAsync((Expression<Func<TransactLinkEntity, bool>> predicate,
                           Func<IQueryable<TransactLinkEntity>, IIncludableQueryable<TransactLinkEntity, object>> include) =>
            {
                return transactLinks.AsQueryable().FirstOrDefault(predicate.Compile());
            });
        }

        private void ConfigereRepositoryToReturnNull()
        {
            _mockRepositoryWrapper
                .Setup(repo => repo.TransactLinksRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TransactLinkEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TransactLinkEntity>, IIncludableQueryable<TransactLinkEntity, object>>>()))
                .ReturnsAsync((TransactLinkEntity?)null);
        }

        private List<TransactLinkEntity> GetTransactLinks() =>
            new()
            {
           new TransactLinkEntity { Id = 1, Url = "URL", UrlTitle = "Title", StreetcodeId = 1 },
           new TransactLinkEntity { Id = 2, Url = "URL2", UrlTitle = "Title2", StreetcodeId = 2 },
            };
    }
}
