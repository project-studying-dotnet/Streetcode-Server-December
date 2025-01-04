using AutoMapper;
using FluentAssertions;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.DAL.Specification;
using Microsoft.EntityFrameworkCore;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.GetAllByTermId
{
    public class GetAllRelatedTermsByTermIdHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllRelatedTermsByTermIdHandler _handler;

        public GetAllRelatedTermsByTermIdHandlerTests()
        {
            this._mockLogger = new Mock<ILoggerService>();
            this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new RelatedTermProfile());
            });
            this._mapper = configuration.CreateMapper();
            this._handler = new GetAllRelatedTermsByTermIdHandler(this._mapper, this._mockRepositoryWrapper.Object, this._mockLogger.Object);
        }

        [Fact]
        public async Task whenGetRelatedTermWithTermId1_thenReturnOKWithRelatedTerms()
        {
            // Arrange
            var relatedTerms = new List<RelatedTerm>
        {
            new RelatedTerm { Id = 1, Word = "TestWord1", TermId = 1 },
            new RelatedTerm { Id = 2, Word = "TestWord2", TermId = 1 },
            new RelatedTerm { Id = 3, Word = "TestWord3", TermId = 2 },
        };

            var relatedTermDTOs = new List<RelatedTermDto>
        {
            new RelatedTermDto { Id = 1, Word = "TestWord1", TermId = 1 },
            new RelatedTermDto { Id = 2, Word = "TestWord2", TermId = 1 },
        };

            this._mockRepositoryWrapper
                .Setup(r => r.RelatedTermRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(relatedTerms.Where(rt => rt.TermId == 1));

            var query = new GetAllRelatedTermsByTermIdQuery(1);

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Count().Should().Be(2);
            result.Value.Should().BeEquivalentTo(relatedTermDTOs);
        }

        [Fact]
        public async Task whenGetRelatedTermWithTermId2_thenReturnOKWithRelatedTerms()
        {
            // Arrange
            var relatedTerms = new List<RelatedTerm>
        {
            new RelatedTerm { Id = 1, Word = "TestWord1", TermId = 1 },
            new RelatedTerm { Id = 2, Word = "TestWord2", TermId = 1 },
            new RelatedTerm { Id = 3, Word = "TestWord3", TermId = 2 },
        };

            var relatedTermDTOs = new List<RelatedTermDto>
        {
            new RelatedTermDto { Id = 3, Word = "TestWord3", TermId = 2 },
        };

            this._mockRepositoryWrapper
                .Setup(r => r.RelatedTermRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(relatedTerms.Where(rt => rt.TermId == 2));

            var query = new GetAllRelatedTermsByTermIdQuery(2);

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Count().Should().Be(1);
            result.Value.Should().BeEquivalentTo(relatedTermDTOs);
        }

        [Fact]
        public async Task whenGetRelatedTermDoNotExist_thenReturnEmptyCollection()
        {
            // Arrange
            var relatedTerms = new List<RelatedTerm>
        {
            new RelatedTerm { Id = 1, Word = "TestWord1", TermId = 1 },
            new RelatedTerm { Id = 2, Word = "TestWord2", TermId = 1 },
            new RelatedTerm { Id = 3, Word = "TestWord3", TermId = 2 },
        };

            this._mockRepositoryWrapper
                .Setup(r => r.RelatedTermRepository.GetAllBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(relatedTerms.Where(rt => rt.TermId == 3));

            var query = new GetAllRelatedTermsByTermIdQuery(3);

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Count().Should().Be(0);
        }
    }
}
