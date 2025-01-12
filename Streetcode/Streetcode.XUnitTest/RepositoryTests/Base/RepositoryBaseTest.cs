using Microsoft.EntityFrameworkCore;
using Moq;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using Streetcode.DAL.Caching.RedisCache;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent;
using Streetcode.DAL.Specification;
using Streetcode.Domain.Entities.Streetcode.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TestEntity = Streetcode.Domain.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.XUnitTest.RepositoryTests.Base
{
    public class RepositoryBaseTest
    {
        private readonly Mock<IRedisCacheService> _mockRedisCacheService;
        private StreetcodeDbContext _dbContext;
        private RepositoryBase<TestEntity> _repository;

        public RepositoryBaseTest()
        {
            _mockRedisCacheService = new Mock<IRedisCacheService>();
        }

        [Fact]
        public async Task GetAllBySpecAsync_ReturnsDataFromCache_WhenCacheExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<StreetcodeDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase1")
                .Options;
            _dbContext = new StreetcodeDbContext(options);

            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = 1, Word = "Entity1", TermId = 1, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "aa", Description = "a" } },
                new TestEntity { Id = 2, Word = "Entity2", TermId = 2, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 2, Title = "aaa", Description = "a" } }
            );
            _dbContext.SaveChanges();

            _repository = new RelatedTermRepository(_dbContext, _mockRedisCacheService.Object);

            var specification = new RelatedTermWithTermSpecification();
            var cachedData = new List<TestEntity>
        {
            new TestEntity 
            {
                Id = 3,
                Word = "CachedEntity",
                TermId = 1,
            }
        };
            _mockRedisCacheService
                .Setup(service => service.GetCachedDataAsync<IEnumerable<TestEntity>>(specification.CacheKey))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _repository.GetAllBySpecAsync(specification);

            // Assert
            Assert.Equal(cachedData, result);
            _mockRedisCacheService.Verify(service => service.GetCachedDataAsync<IEnumerable<TestEntity>>(specification.CacheKey), Times.Once);
            _mockRedisCacheService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAllBySpecAsync_SetsCache_WhenCacheDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<StreetcodeDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;
            _dbContext = new StreetcodeDbContext(options);

            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = 1, Word = "Entity1", TermId = 1, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "aa", Description = "a" } },
                new TestEntity { Id = 2, Word = "Entity2", TermId = 2, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 2, Title = "aaa", Description = "a" } }
            );
            _dbContext.SaveChanges();

            _repository = new RelatedTermRepository(_dbContext, _mockRedisCacheService.Object);

            var specification = new RelatedTermWithTermSpecification();
            _mockRedisCacheService
                .Setup(service => service.GetCachedDataAsync<IEnumerable<TestEntity>>(specification.CacheKey))
                .ReturnsAsync((IEnumerable<TestEntity>?)null);

            // Act
            var result = await _repository.GetAllBySpecAsync(specification);

            // Assert
            Assert.Equal(2, result.Count());
            _mockRedisCacheService.Verify(service => service.GetCachedDataAsync<IEnumerable<TestEntity>>(specification.CacheKey), Times.Once);
            _mockRedisCacheService.Verify(service => service.SetCachedDataAsync(specification.CacheKey, It.IsAny<IEnumerable<TestEntity>>(), specification.CacheMinutes), Times.Once);
        }

        [Fact]
        public async Task GetFirstOrDefaultBySpecAsync_ReturnsDataFromCache_WhenCacheExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<StreetcodeDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;
            _dbContext = new StreetcodeDbContext(options);

            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = 1, Word = "Entity1", TermId = 1, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "aa", Description = "a" } },
                new TestEntity { Id = 2, Word = "Entity2", TermId = 2, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 2, Title = "aaa", Description = "a" } }
            );
            _dbContext.SaveChanges();

            _repository = new RelatedTermRepository(_dbContext, _mockRedisCacheService.Object);

            var specification = new RelatedTermWithTermSpecification();
            var cachedData = new TestEntity { Id = 3, Word = "CachedEntity", TermId = 1 };
            _mockRedisCacheService
                .Setup(service => service.GetCachedDataAsync<TestEntity>(specification.CacheKey))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _repository.GetFirstOrDefaultBySpecAsync(specification);

            // Assert
            Assert.Equal(cachedData, result);
            _mockRedisCacheService.Verify(service => service.GetCachedDataAsync<TestEntity>(specification.CacheKey), Times.Once);
        }

        [Fact]
        public async Task GetFirstOrDefaultBySpecAsync_SetsCache_WhenCacheDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<StreetcodeDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase4")
                .Options;
            _dbContext = new StreetcodeDbContext(options);

            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = 1, Word = "Entity1", TermId = 1, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "aa", Description = "a" } },
                new TestEntity { Id = 2, Word = "Entity2", TermId = 2, Term = new DAL.Entities.Streetcode.TextContent.Term { Id = 2, Title = "aaa", Description = "a" } }
            );
            _dbContext.SaveChanges();

            _repository = new RelatedTermRepository(_dbContext, _mockRedisCacheService.Object);

            var specification = new RelatedTermWithTermSpecification();
            _mockRedisCacheService
                .Setup(service => service.GetCachedDataAsync<TestEntity>(specification.CacheKey))
                .ReturnsAsync((TestEntity?)null);

            // Act
            var result = await _repository.GetFirstOrDefaultBySpecAsync(specification);

            // Assert
            Assert.NotNull(result);
            _mockRedisCacheService.Verify(service => service.GetCachedDataAsync<TestEntity>(specification.CacheKey), Times.Once);
            _mockRedisCacheService.Verify(service => service.SetCachedDataAsync(specification.CacheKey, It.IsAny<TestEntity>(), specification.CacheMinutes), Times.Once);
        }
    }
}
