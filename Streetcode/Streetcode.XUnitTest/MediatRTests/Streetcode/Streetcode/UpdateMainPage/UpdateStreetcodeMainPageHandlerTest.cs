﻿using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Streetcode;
using Streetcode.BLL.Mapping.Streetcode.Types;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.UpdateMainPage;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.Audio;
using Streetcode.BLL.Services.Image;
using Streetcode.BLL.Specifications.Streetcode;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Streetcode.UpdateMainPage
{
    public class UpdateStreetcodeMainPageHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<IImageService> _imageServiceMock = new();
        private readonly Mock<IAudioService> _audioServiceMock = new();
        private readonly UpdateStreetcodeMainPageHandler _handler;

        public UpdateStreetcodeMainPageHandlerTest()
        {
            _handler = new UpdateStreetcodeMainPageHandler(
                _repositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _imageServiceMock.Object,
                _audioServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Id = 1,
                Title = "Updated Title",
                StreetcodeType = StreetcodeType.Person,
                TransliterationUrl = "",
                Images = new List<ImageFileBaseCreateDto>
            {
                new ImageFileBaseCreateDto { BaseFormat = "image1" },
                new ImageFileBaseCreateDto { BaseFormat = "image2" }
            },
                Audio = new AudioFileBaseCreateDto { BaseFormat = "audio1" }
            });

            var existingMainPage = new PersonStreetcode
            {
                Id = 1,
                Title = "Old Title",
                Images = new List<Image> { new Image { BlobName = "oldImage" } },
                Audio = new Audio { BlobName = "oldAudio" }
            };

            _repositoryMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<StreetcodeMainPageSpecification>()))
                .ReturnsAsync(existingMainPage);

            _mapperMock
                .Setup(mapper => mapper.Map<PersonStreetcode>(It.IsAny<StreetcodeMainPageUpdateDto>()))
                .Returns(new PersonStreetcode { Id = 1, Title = "Updated Title" });

            _repositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _imageServiceMock.Verify(service => service.DeleteImage("oldImage"), Times.Once);
            _audioServiceMock.Verify(service => service.DeleteAudio("oldAudio"), Times.Once);
            _repositoryMock.Verify(repo => repo.StreetcodeRepository.Update(It.IsAny<PersonStreetcode>()), Times.Once);
            _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_MainPageNotFound_ReturnsFailResult()
        {
            // Arrange
            var command = new UpdateStreetcodeMainPageCommand(new StreetcodeMainPageUpdateDto
            {
                Id = 1,
                Title = "Updated Title",
                TransliterationUrl = "",
                Images = new List<ImageFileBaseCreateDto> { }
            });

            _repositoryMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultBySpecAsync(It.IsAny<StreetcodeMainPageSpecification>()))
                .ReturnsAsync((PersonStreetcode)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
    }
}
