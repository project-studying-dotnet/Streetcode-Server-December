using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.Domain.Entities.Media.Images;
using Streetcode.Domain.Entities.Streetcode;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.ArtTests
{
    public class GetArtByStreetcodeIdHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IBlobService> _blobMock;
        private readonly GetArtsByStreetcodeIdHandler _getArtByStreetcodeIdHandler;

        public GetArtByStreetcodeIdHandlerTest()
        {
            _mapperMock = new();
            _repositoryMock = new();
            _loggerMock = new();
            _blobMock = new();
            _getArtByStreetcodeIdHandler = new(_repositoryMock.Object, _mapperMock.Object, _blobMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnArtByStreetcodeId_WhenArtExists()
        {
            // A(Arrange):

            int streetcodeId = 10;

            var allArtsByStreetcodeId = new List<Art>()
            {
                new Art 
                {
                    Id = 1, 
                    Description = "None1", 
                    Title = "Art_0", 
                    ImageId = 11, 
                    Image = new Image { Id = 11, BlobName = "blobNum1" },  
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } }
                },
                new Art 
                {
                    Id = 2, 
                    Description = "None2", 
                    Title = "Art_1", 
                    ImageId = 12, 
                    Image = new Image { Id = 12, BlobName = "blobNum2" },  
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } }
                }, 
                new Art 
                {
                    Id = 3, 
                    Description = "None3", 
                    Title = "Art_2", 
                    ImageId = 13,
                    Image = new Image { Id = 13, BlobName = "blobNum3" }, 
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } } 
                },
                new Art 
                {
                    Id = 4, 
                    Description = "None4", 
                    Title = "Art_3", 
                    ImageId = 14, 
                    Image = new Image { Id = 14, BlobName = "blobNum4" },  
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } }
                }
            };

            var allDtosByStreetcodeId = new List<ArtDto>()
            {
                new ArtDto 
                {
                    Id = 1,
                    Description = "None1",
                    Title = "Art_0",
                    ImageId = 11, 
                    Image = new ImageDto { Id = 11, BlobName = "blobNum1" }
                },
                new ArtDto 
                {
                    Id = 2, 
                    Description = "None2", 
                    Title = "Art_1", 
                    ImageId = 12, 
                    Image = new ImageDto { Id = 12, BlobName = "blobNum2" }
                },
                new ArtDto 
                {
                    Id = 3,
                    Description = "None3",
                    Title = "Art_2", 
                    ImageId = 13, 
                    Image = new ImageDto { Id = 13, BlobName = "blobNum3" } 
                },
                new ArtDto 
                {
                    Id = 4,
                    Description = "None4", 
                    Title = "Art_3", 
                    ImageId = 14, 
                    Image = new ImageDto { Id = 14, BlobName = "blobNum4" }
                }
            };

            _repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.Is<Expression<Func<Art, bool>>>(predicate => predicate.Compile()(allArtsByStreetcodeId[0])), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())).ReturnsAsync(allArtsByStreetcodeId);
            _mapperMock.Setup(m => m.Map<IEnumerable<ArtDto>>(allArtsByStreetcodeId)).Returns(allDtosByStreetcodeId);
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum1")).Returns(Task.FromResult("base64_blobNum1"));
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum2")).Returns(Task.FromResult("base64_blobNum2"));
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum3")).Returns(Task.FromResult("base64_blobNum3"));
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum4")).Returns(Task.FromResult("base64_blobNum4"));

            // (Act):

            var res = await _getArtByStreetcodeIdHandler.Handle(new GetArtsByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

            // (Assert):

            Assert.True(res.IsSuccess);
            Assert.Equal(allDtosByStreetcodeId.Count, res.Value.Count());
            Assert.Collection(res.Value,
               artDto => Assert.Equal("base64_blobNum1", artDto.Image.Base64),
               artDto => Assert.Equal("base64_blobNum2", artDto.Image.Base64),
               artDto => Assert.Equal("base64_blobNum3", artDto.Image.Base64),
               artDto => Assert.Equal("base64_blobNum4", artDto.Image.Base64));

            _repositoryMock.Verify(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<ArtDto>>(allArtsByStreetcodeId), Times.Once);
            _blobMock.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Exactly(allArtsByStreetcodeId.Count));
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenStreetcodeIdDoesntExists()
        {
            // (Arrange):

            int incorrectStreetcodeId = 1000;

            _repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())).ReturnsAsync(null as IEnumerable<Art>);

            // (Act):

            var res = await _getArtByStreetcodeIdHandler.Handle(new GetArtsByStreetcodeIdQuery(incorrectStreetcodeId), CancellationToken.None);

            // (Assert):

            // Assert.Equal($"Cannot find a art by streetcode id: {incorrectStreetcodeId}", res.Errors[0].Message);
            Assert.True(res.IsFailed);
            Assert.Single(res.Errors);


            _repositoryMock.Verify(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()), Times.Once);
            
            // _loggerMock.Verify(l => l.LogError(new GetArtsByStreetcodeIdQuery(incorrectStreetcodeId), $"Cannot find a art by streetcode id: {incorrectStreetcodeId}"), Times.Once);
        }
    }
}
