using AutoMapper;
using FluentAssertions;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
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

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.GetById
{
    public class GetRelatedTermByIdHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetRelatedTermByIdHandler _handler;

        public GetRelatedTermByIdHandlerTests()
        {
            this._mockLogger = new Mock<ILoggerService>();
            this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new RelatedTermProfile());
            });
            this._mapper = configuration.CreateMapper();
            this._handler = new GetRelatedTermByIdHandler(this._mapper, this._mockRepositoryWrapper.Object, this._mockLogger.Object);
        }

        private void CreateRepository()
        {
            var terms = new List<Term>
        {
            new Term { Id = 1, Title = "Term1", Description = "Description1" },
            new Term { Id = 2, Title = "Term2", Description = "Description2" },
        };

            var relatedTerms = new List<RelatedTerm>
        {
            new RelatedTerm { Id = 1, Word = "Hello", TermId = 1, Term = terms[0] },
            new RelatedTerm { Id = 2, Word = "HelloelloH", TermId = 1, Term = terms[0] },
            new RelatedTerm { Id = 3, Word = "He", TermId = 2, Term = terms[1] },
        };


            this._mockRepositoryWrapper
                .Setup(repo => repo.RelatedTermRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<RelatedTerm, bool>>>(),
                    It.IsAny<Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>?>()))
                .ReturnsAsync((
                    Expression<Func<RelatedTerm, bool>> predicate,
                    Func<IQueryable<RelatedTerm>, IIncludableQueryable<RelatedTerm, object>>? include) =>
                {
                    var query = relatedTerms.AsQueryable();
                    if (predicate != null)
                    {
                        query = query.Where(predicate);
                    }

                    if (include != null)
                    {
                        query = include(query);
                    }

                    return query.FirstOrDefault();
                });
        }

        [Fact]
        public async Task whenGetRelatedTermWithId1_thenReturnOKWithRelatedTerm()
        {
            this.CreateRepository();
            var result = await this._handler.Handle(new GetRelatedTermByIdQuery(1), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<RelatedTermDTO>();
            result.Value.Id.Should().Be(1);
        }

        [Fact]
        public async Task whenGetRelatedTermWithId2_thenReturnOKWithRelatedTerm()
        {
            this.CreateRepository();
            var result = await this._handler.Handle(new GetRelatedTermByIdQuery(2), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<RelatedTermDTO>();
            result.Value.Id.Should().Be(2);
        }

        [Fact]
        public async Task whenGetRelatedTermDoNotExist_thenReturnException()
        {
            this.CreateRepository();
            var result = await this._handler.Handle(new GetRelatedTermByIdQuery(10), CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("Cannot get word by id");
        }
    }
}
