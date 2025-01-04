using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Image.Create
{
    public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<ImageDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IImageService _imageService;
        private readonly ILoggerService _logger;

        public CreateImageHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILoggerService logger,
            IImageService imageService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
        }

        public async Task<Result<ImageDto>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
        {
            DAL.Entities.Media.Images.Image image = _imageService.ConfigureImage(request.Image);

            await _repositoryWrapper.ImageRepository.CreateAsync(image);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            var createdImage = _mapper.Map<ImageDto>(image);

            createdImage.Base64 = _imageService.ImageBase64(createdImage);

            if (resultIsSuccess)
            {
                return Result.Ok(createdImage);
            }
            else
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "image");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}