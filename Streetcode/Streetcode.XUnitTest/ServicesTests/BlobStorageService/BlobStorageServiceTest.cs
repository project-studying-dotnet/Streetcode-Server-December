using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Moq;
using Streetcode.BLL.Services.BlobStorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Azure;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Dasync.Collections;
using Streetcode.Domain.Entities.Media;
using Streetcode.Domain.Entities.Media.Images;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.ServicesTests.BlobStorageService
{
    public class BlobStorageServiceTest
    {
        private readonly Mock<BlobServiceClient> _mockBlobServiceClient;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<BlobContainerClient> _mockContainerClient;
        private readonly Mock<BlobClient> _mockBlobClient;

        private readonly BlobService _blobService;

        public BlobStorageServiceTest()
        {
            _mockBlobServiceClient = new Mock<BlobServiceClient>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockContainerClient = new Mock<BlobContainerClient>();
            _mockBlobClient = new Mock<BlobClient>();

            var mockEnvVariables = Options.Create(new BlobEnvironmentVariables
            {
                BlobStoreKey = "SlavaKasterovSuperGoodInshalaKey",
                BlobStorePath = "path/"
            });

            _mockBlobServiceClient
                .Setup(x => x.GetBlobContainerClient("streetcode"))
                .Returns(_mockContainerClient.Object);

            _mockContainerClient
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlobClient.Object);

            _blobService = new BlobService(mockEnvVariables, _mockBlobServiceClient.Object, _mockRepositoryWrapper.Object);
        }

        [Fact]
        public async Task SaveFileInStorageBase64_ShouldEncryptAndUploadFile()
        {
            // Arrange
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test Image Content"));
            string name = "image";
            string extension = "png";

            _mockBlobClient
                .Setup(x => x.UploadAsync(It.IsAny<Stream>()))
                .Returns(Task.FromResult((Response<BlobContentInfo>)null!));

            _mockContainerClient
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlobClient.Object);

            // Act
            await _blobService.SaveFileInStorageBase64(base64, name, extension);

            // Assert
            _mockBlobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>()), Times.Once);
        }

        [Fact]
        public async Task DeleteFileInStorage_ShouldCallDeleteIfExists()
        {
            // Arrange
            string name = "image.png";

            _mockBlobClient
                .Setup(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default))
                .ReturnsAsync(Response.FromValue(true, null!));

            _mockContainerClient
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlobClient.Object);

            // Act
            await _blobService.DeleteFileInStorage(name);

            // Assert
            _mockBlobClient.Verify(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default), Times.Once);
        }

        [Fact]
        public async Task CleanBlobStorage_ShouldDeleteUnusedFiles()
        {
            // Arrange
            var audios = new List<Audio>
            {
                new Audio { Id = 1, Title = "Audio1", BlobName = "blob1" },
                new Audio { Id = 2, Title = "Audio2", BlobName = "blob2" },
                new Audio { Id = 3, Title = "Audio3", BlobName = "blob3" }
            };

            var images = new List<Image>
            {
                new Image { Id = 1, BlobName = "blob4" },
                new Image { Id = 2, BlobName = "blob5" },
            };

            _mockRepositoryWrapper
                .Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<Expression<Func<Image, bool>>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(images);

            _mockRepositoryWrapper
                .Setup(r => r.AudioRepository.GetAllAsync(It.IsAny<Expression<Func<Audio, bool>>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(audios);

            var emptyAsyncPageable = AsyncPageable<BlobItem>.FromPages(new List<Page<BlobItem>>());

            _mockContainerClient
                .Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), default))
                .Returns(emptyAsyncPageable);

            // Act
            await _blobService.CleanBlobStorage();

            // Assert
            _mockBlobClient.Verify(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, default), Times.Never);
        }
    }
}
