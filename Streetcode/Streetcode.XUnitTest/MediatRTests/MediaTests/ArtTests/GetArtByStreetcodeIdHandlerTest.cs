using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
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
            _getArtByStreetcodeIdHandler = new(_repositoryMock.Object, _mapperMock.Object, _blobMock.Object , _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnArtByStreetcodeId_WhenArtExists()
        {
            //A(Arrange):

            int streetcodeId = 10;

            var allArtsByStreetcodeId = new List<Art>()
            {
                new Art 
                {
                    Id = 1, 
                    Description = "None1", 
                    Title = "Art_0", 
                    ImageId = 11, 
                    Image = new Image { Id = 11, BlobName = "blobNum1"},  
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } }
                },
                new Art 
                {
                    Id = 2, 
                    Description = "None2", 
                    Title = "Art_1", 
                    ImageId = 12, 
                    Image = new Image { Id = 12, BlobName = "blobNum2"},  
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } }
                }, 
                new Art 
                {
                    Id = 3 , 
                    Description = "None3" , 
                    Title = "Art_2" , 
                    ImageId = 13, 
                    Image = new Image { Id = 13, BlobName = "blobNum3"}, 
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId } } 
                },
                new Art 
                {
                    Id = 4, 
                    Description = "None4", 
                    Title = "Art_3", 
                    ImageId = 14, 
                    Image = new Image { Id = 14, BlobName = "blobNum4" },  
                    StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = streetcodeId} }
                }
            };

            var allDtosByStreetcodeId = new List<ArtDTO>()
            {
                new ArtDTO 
                {
                    Id = 1 ,
                    Description = "None1" ,
                    Title = "Art_0" ,
                    ImageId = 11, 
                    Image = new ImageDTO { Id = 11, BlobName = "blobNum1" }
                },
                new ArtDTO 
                {
                    Id = 2 , 
                    Description = "None2" , 
                    Title = "Art_1" , 
                    ImageId = 12, 
                    Image = new ImageDTO { Id = 12, BlobName = "blobNum2" }
                },
                new ArtDTO 
                {
                    Id = 3 ,
                    Description = "None3" ,
                    Title = "Art_2" , 
                    ImageId = 13, 
                    Image = new ImageDTO { Id = 13, BlobName = "blobNum3" } 
                },
                new ArtDTO 
                {
                    Id = 4 ,
                    Description = "None4" , 
                    Title = "Art_3" , 
                    ImageId = 14, 
                    Image = new ImageDTO { Id = 14, BlobName = "blobNum4" }
                }
            };

            _repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.Is<Expression<Func<Art, bool>>>(predicate => predicate.Compile()(allArtsByStreetcodeId[0])), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())).ReturnsAsync(allArtsByStreetcodeId);
            _mapperMock.Setup(m => m.Map<IEnumerable<ArtDTO>>(allArtsByStreetcodeId)).Returns(allDtosByStreetcodeId);
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum1")).Returns("base64_blobNum1");
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum2")).Returns("base64_blobNum2");
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum3")).Returns("base64_blobNum3");
            _blobMock.Setup(b => b.FindFileInStorageAsBase64("blobNum4")).Returns("base64_blobNum4");

            //A(Act):

            var res = await _getArtByStreetcodeIdHandler.Handle(new GetArtsByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

            //A(Assert):

            Assert.True(res.IsSuccess);
            Assert.Equal(allDtosByStreetcodeId.Count, res.Value.Count());
            Assert.Collection(res.Value,
               artDto => Assert.Equal("base64_blobNum1", artDto.Image.Base64),
               artDto => Assert.Equal("base64_blobNum2", artDto.Image.Base64),
               artDto => Assert.Equal("base64_blobNum3", artDto.Image.Base64),
               artDto => Assert.Equal("base64_blobNum4", artDto.Image.Base64)
            );

            _repositoryMock.Verify(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<ArtDTO>>(allArtsByStreetcodeId), Times.Once);
            _blobMock.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Exactly(allArtsByStreetcodeId.Count));
        }


        [Fact]
        public async Task Handle_ShouldReturnFail_WhenStreetcodeIdDoesntExists()
        {
            //A(Arrange):

            int incorrectStreetcodeId = 1000;

            _repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())).ReturnsAsync(null as IEnumerable<Art>);
            //_repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.Is<Expression<Func<Art, bool>>>(predicate => predicate.Compile()(new Art {StreetcodeArts = new List<StreetcodeArt> { new StreetcodeArt { StreetcodeId = incorrectStreetcodeId} } }) == false), null)).ReturnsAsync(null as IEnumerable<Art>);

            //A(Act):

            var res = await _getArtByStreetcodeIdHandler.Handle(new GetArtsByStreetcodeIdQuery(incorrectStreetcodeId), CancellationToken.None);

            //A(Assert):

            Assert.True(res.IsFailed);
            Assert.Single(res.Errors);
            Assert.Equal($"Cannot find any art with corresponding streetcode id: {incorrectStreetcodeId}", res.Errors[0].Message);

            _repositoryMock.Verify(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()), Times.Once);
            _loggerMock.Verify(l => l.LogError(new GetArtsByStreetcodeIdQuery(incorrectStreetcodeId), $"Cannot find any art with corresponding streetcode id: {incorrectStreetcodeId}"), Times.Once);
        }
    }
}
