using AutoMapper;
using FluentAssertions;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll;
using Streetcode.BLL.MediatR.Toponyms.GetById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTermTests.GetAll;

public class GetAllRelatedTermsHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllRelatedTermsHandler _handler;

    public GetAllRelatedTermsHandlerTests()
    {
        this._mockLogger = new Mock<ILoggerService>();
        this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new RelatedTermProfile());
        });
        this._mapper = configuration.CreateMapper();
        this._handler = new GetAllRelatedTermsHandler(this._mapper, this._mockRepositoryWrapper.Object, this._mockLogger.Object);
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
       .Setup(repo => repo.RelatedTermRepository.GetAllAsync(
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

           return query.ToList();
       });
    }

    private void CreateEmptyRepository()
    {
        var relatedTerms = new List<RelatedTerm>();
        this._mockRepositoryWrapper
               .Setup(repo => repo.RelatedTermRepository.GetAllAsync(
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

                   return query.ToList();
               });
    }

    [Fact]
    public async Task whenGetAllRequestAndThereAreRelatedTerms_thenReturnOKWithRelatedTerms()
    {
        this.CreateRepository();
        var result = await this._handler.Handle(new GetAllRelatedTermsQuery(), CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().BeOfType<List<RelatedTermDTO>>();
        result.Value.Count().Should().Be(3);
    }

    [Fact]
    public async Task whenGetAllRequestAndThereAreNoRelatedTerms_thenReturnEmptyList()
    {
        this.CreateEmptyRepository();
        var result = await this._handler.Handle(new GetAllRelatedTermsQuery(), CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        result.Value.Should().BeOfType<List<RelatedTermDTO>>();
    }
}
