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
using Streetcode.DAL.Specification;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;

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

        [Fact]
        public async Task whenGetRelatedTermWithId1_thenReturnOKWithRelatedTerm()
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
                .Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(relatedTerms.Where(rt => rt.Id == 1).FirstOrDefault());
            var result = await this._handler.Handle(new GetRelatedTermByIdQuery(1), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<RelatedTermDTO>();
            result.Value.Id.Should().Be(1);
            result.Value.Word.Should().Be("Hello");
        }

        [Fact]
        public async Task whenGetRelatedTermWithId2_thenReturnOKWithRelatedTerm()
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
                .Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(relatedTerms.Where(rt => rt.Id == 2).FirstOrDefault());
            var result = await this._handler.Handle(new GetRelatedTermByIdQuery(1), CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<RelatedTermDTO>();
            result.Value.Id.Should().Be(2);
            result.Value.Word.Should().Be("HelloelloH");
        }

        [Fact]
        public async Task whenGetRelatedTermDoNotExist_thenReturnException()
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
                .Setup(r => r.RelatedTermRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<IBaseSpecification<RelatedTerm>>()))
                .ReturnsAsync(relatedTerms.Where(rt => rt.Id == 10).FirstOrDefault());
            var result = await this._handler.Handle(new GetRelatedTermByIdQuery(10), CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("Cannot find a word with corresponding id: 10");
        }
    }
}
