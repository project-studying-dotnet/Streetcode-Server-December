using Moq;
using Streetcode.DAL.Specification.Evaluator;
using Streetcode.DAL.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Streetcode.Domain.Entities.Streetcode.TextContent;

namespace Streetcode.XUnitTest.SpecificationTests
{
    public class SpecificationEvaluatorTests
    {
        private readonly IQueryable<RelatedTerm> _queryable;

        public SpecificationEvaluatorTests()
        {
            var terms = new List<Term>()
            {
                new Term { Id = 1, Title = "Title1", Description = "a" },
                new Term { Id = 2, Title = "Title2", Description = "aa" },
            };

            var data = new List<RelatedTerm>
            {
                new RelatedTerm { Id = 1, Word = "bTerm1", TermId = 1, Term = terms[0] },
                new RelatedTerm { Id = 2, Word = "aTerm2", TermId = 1, Term = terms[0] },
                new RelatedTerm { Id = 3, Word = "dTerm3", TermId = 2, Term = terms[1] },
                new RelatedTerm { Id = 4, Word = "cTerm4", TermId = 2, Term = terms[1] },
            }.AsQueryable();

            this._queryable = data;
        }

        [Fact]
        public void GetQuery_NoSpecification_ReturnsAllEntities()
        {
            // Arrange
            var specificationMock = new Mock<IBaseSpecification<RelatedTerm>>();
            specificationMock.Setup(s => s.Predicate).Returns((Expression<Func<RelatedTerm, bool>>)null);
            specificationMock.Setup(s => s.Includes).Returns(new List<Expression<Func<RelatedTerm, object>>> { x => x.Term, });
            specificationMock.Setup(s => s.OrderBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.OrderByDescending).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.GroupBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.IsPagingEnabled).Returns(false);

            // Act
            var result = SpecificationEvaluator<RelatedTerm>.GetQuery(this._queryable, specificationMock.Object);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Equal(this._queryable, result);
            Assert.Equal("Title1", result.First().Term.Title);
        }

        [Fact]
        public void GetQuery_WithPredicate_FiltersEntities()
        {
            // Arrange
            var specificationMock = new Mock<IBaseSpecification<RelatedTerm>>();
            specificationMock.Setup(s => s.Predicate).Returns(x => x.TermId == 1);
            specificationMock.Setup(s => s.Includes).Returns(new List<Expression<Func<RelatedTerm, object>>> { x => x.Term, });
            specificationMock.Setup(s => s.OrderBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.OrderByDescending).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.GroupBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.IsPagingEnabled).Returns(false);

            // Act
            var result = SpecificationEvaluator<RelatedTerm>.GetQuery(this._queryable, specificationMock.Object);

            // Assert
            Assert.Equal(2, result.Count());
            foreach (var item in result)
            {
                item.TermId.Should().Be(1);
            }
        }

        [Fact]
        public void GetQuery_WithOrderBy_SortsEntities()
        {
            // Arrange
            var specificationMock = new Mock<IBaseSpecification<RelatedTerm>>();
            specificationMock.Setup(s => s.Predicate).Returns((Expression<Func<RelatedTerm, bool>>)null);
            specificationMock.Setup(s => s.Includes).Returns(new List<Expression<Func<RelatedTerm, object>>> { x => x.Term, });
            specificationMock.Setup(s => s.OrderBy).Returns(x => x.Word);
            specificationMock.Setup(s => s.OrderByDescending).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.GroupBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.IsPagingEnabled).Returns(false);

            // Act
            var result = SpecificationEvaluator<RelatedTerm>.GetQuery(this._queryable, specificationMock.Object).ToList();

            // Assert
            Assert.Equal("aTerm2", result.First().Word);
            Assert.Equal("dTerm3", result.Last().Word);
        }

        [Fact]
        public void GetQuery_WithOrderByDescending_SortsEntities()
        {
            // Arrange
            var specificationMock = new Mock<IBaseSpecification<RelatedTerm>>();
            specificationMock.Setup(s => s.Predicate).Returns((Expression<Func<RelatedTerm, bool>>)null);
            specificationMock.Setup(s => s.Includes).Returns(new List<Expression<Func<RelatedTerm, object>>> { x => x.Term, });
            specificationMock.Setup(s => s.OrderBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.OrderByDescending).Returns(x => x.Word);
            specificationMock.Setup(s => s.GroupBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.IsPagingEnabled).Returns(false);

            // Act
            var result = SpecificationEvaluator<RelatedTerm>.GetQuery(this._queryable, specificationMock.Object).ToList();

            // Assert
            Assert.Equal("dTerm3", result.First().Word);
            Assert.Equal("aTerm2", result.Last().Word);
        }

        [Fact]
        public void GetQuery_WithPaging_ReturnsPagedResults()
        {
            // Arrange
            // Arrange
            var specificationMock = new Mock<IBaseSpecification<RelatedTerm>>();
            specificationMock.Setup(s => s.Predicate).Returns((Expression<Func<RelatedTerm, bool>>)null);
            specificationMock.Setup(s => s.Includes).Returns(new List<Expression<Func<RelatedTerm, object>>> { x => x.Term, });
            specificationMock.Setup(s => s.OrderBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.OrderByDescending).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.GroupBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.IsPagingEnabled).Returns(true);
            specificationMock.Setup(s => s.Skip).Returns(2);
            specificationMock.Setup(s => s.Take).Returns(2);

            // Act
            var result = SpecificationEvaluator<RelatedTerm>.GetQuery(this._queryable, specificationMock.Object).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("dTerm3", result.First().Word);
            Assert.Equal("cTerm4", result.Last().Word);
        }

        [Fact]
        public void GetQuery_WithGroupBy_GroupsEntities()
        {
            // Arrange
            var specificationMock = new Mock<IBaseSpecification<RelatedTerm>>();
            specificationMock.Setup(s => s.Predicate).Returns((Expression<Func<RelatedTerm, bool>>)null);
            specificationMock.Setup(s => s.Includes).Returns(new List<Expression<Func<RelatedTerm, object>>> { x => x.Term, });
            specificationMock.Setup(s => s.OrderBy).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.OrderByDescending).Returns((Expression<Func<RelatedTerm, object>>)null);
            specificationMock.Setup(s => s.GroupBy).Returns(x => x.TermId);
            specificationMock.Setup(s => s.IsPagingEnabled).Returns(false);

            // Act
            var result = SpecificationEvaluator<RelatedTerm>.GetQuery(this._queryable, specificationMock.Object).ToList();

            // Assert
            Assert.Equal(4, result.Count);
        }
    }
}
