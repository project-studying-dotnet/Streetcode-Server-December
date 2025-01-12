using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using Streetcode.Domain.Entities.Streetcode.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RedisCacheServiceClass = Streetcode.DAL.Caching.RedisCache.RedisCacheService;

namespace Streetcode.XUnitTest.ServicesTests.RedisCacheService
{
    public class RedisCacheServiceTest
    {
        public class RedisCacheServiceTests
        {
            private readonly Mock<IDistributedCache> _mockCache;
            private readonly RedisCacheServiceClass _redisCacheService;

            public RedisCacheServiceTests()
            {
                _mockCache = new Mock<IDistributedCache>();
                _redisCacheService = new RedisCacheServiceClass(_mockCache.Object);
            }

            [Fact]
            public async Task GetCachedDataAsync_ShouldReturnData_WhenDataIsInCache()
            {
                // Arrange
                var expectedData = new RelatedTerm()
                {
                    Id = 1,
                    Word = "test",
                    TermId = 1,
                };

                var serializedData = JsonConvert.SerializeObject(expectedData);
                var byteData = Encoding.UTF8.GetBytes(serializedData);

                _mockCache.Setup(m => m.GetAsync("some_key", default)).ReturnsAsync(byteData);

                // Act
                var result = await _redisCacheService.GetCachedDataAsync<RelatedTerm>("some_key");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expectedData.Id, result.Id);
                Assert.Equal(expectedData.Word, result.Word);
                Assert.Equal(expectedData.TermId, result.TermId);
            }

            [Fact]
            public async Task GetCachedDataAsync_ShouldReturnDefault_WhenDataIsNotInCache()
            {
                // Arrange
                _mockCache.Setup(m => m.GetAsync("some_key", default)).ReturnsAsync((byte[])null!);

                // Act
                var result = await _redisCacheService.GetCachedDataAsync<object>("some_key");

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task SetCachedDataAsync_ShouldSetDataInCache()
            {
                // Arrange
                var data = new { Id = 1, Word = "Test" };
                var serializedData = JsonConvert.SerializeObject(data);
                var byteData = Encoding.UTF8.GetBytes(serializedData);

                // Act
                await _redisCacheService.SetCachedDataAsync("some_key", data, 10);

                // Assert
                _mockCache.Verify(m => m.SetAsync("some_key", byteData, It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
            }

            [Fact]
            public async Task RemoveDataAsync_ShouldRemoveDataFromCache()
            {
                // Act
                await _redisCacheService.RemoveDataAsync("some_key");

                // Assert
                _mockCache.Verify(m => m.RemoveAsync("some_key", default), Times.Once);
            }
        }
    }
}
