using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Transactions;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using TransactLinkEntity = Streetcode.DAL.Entities.Transactions.TransactionLink;

namespace Streetcode.XUnitTest.MediatRTests.Transactions.TransactionLink.GetAllTransactLinksHandlerTests
{
    public class GetAllTransactLinksHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllTransactLinksHandler _handler;

        public GetAllTransactLinksHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(typeof(TransactionLinkProfile)));
            _mapper = cfg.CreateMapper();
            _handler = new GetAllTransactLinksHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResultWithDTOs_WhenDataExists()
        {
            // Arrange
            var transactLinks = GetTransactLinks();
            ConfigureRepositoryToReturn(transactLinks);

            // Act
            var result = await _handler.Handle(new GetAllTransactLinksQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty().And.HaveCount(transactLinks.Count);
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResultAndLogError_WhenDataIsNull()
        {
            // Arrange
            ConfigureRepositoryToReturn(null!);

            // Act
            var result = await _handler.Handle(new GetAllTransactLinksQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(error => error.Message.Contains("transaction link"));

            _mockLogger.Verify(logger => logger.LogError(It.IsAny<object>(), It.Is<string>(msg => msg.Contains("transaction link"))), Times.Once);
        }

        private void ConfigureRepositoryToReturn(List<TransactLinkEntity> transactLinks)
        {
            _mockRepositoryWrapper
                .Setup(repo => repo.TransactLinksRepository.GetAllAsync(
                    It.IsAny<Expression<Func<TransactLinkEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TransactLinkEntity>, IIncludableQueryable<TransactLinkEntity, object>>>()))
                .ReturnsAsync(transactLinks);
        }

        private List<TransactLinkEntity> GetTransactLinks() =>
            new()
            {
                new TransactLinkEntity { Id = 1, Url = "URL", UrlTitle = "Title", StreetcodeId = 1 },
                new TransactLinkEntity { Id = 2, Url = "URL2", UrlTitle = "Title2", StreetcodeId = 2 },
            };

        private List<TransactLinkDto> GetTransactLinkDTOs() =>
            new()
            {
                new TransactLinkDto { Id = 1, Url = "URL", QrCodeUrl = "QrCodeUrl", StreetcodeId = 1 },
                new TransactLinkDto { Id = 2, Url = "URL2", QrCodeUrl = "QrCodeUrl2", StreetcodeId = 2 },
            };
    }
}
