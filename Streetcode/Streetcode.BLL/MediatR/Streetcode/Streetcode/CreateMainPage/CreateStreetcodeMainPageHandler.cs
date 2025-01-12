using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Audio;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.Domain.Entities.Streetcode;
using Streetcode.Domain.Entities.Streetcode.Types;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage
{
    public class CreateStreetcodeMainPageHandler : IRequestHandler<CreateStreetcodeMainPageCommand, Result<StreetcodeDto>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IImageService _imageService;
        private readonly IAudioService _audioService;

        public CreateStreetcodeMainPageHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger, IImageService imageService, IAudioService audioService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
            _audioService = audioService;
        }

        public async Task<Result<StreetcodeDto>> Handle(CreateStreetcodeMainPageCommand request, CancellationToken cancellationToken)
        {
            if (request.StreetcodeMainPage is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "main page block");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            StreetcodeContent mainPage;

            if (request.StreetcodeMainPage.StreetcodeType == Streetcode.Domain.Enums.StreetcodeType.Person)
            {
                mainPage = _mapper.Map<PersonStreetcode>(request.StreetcodeMainPage);
            }
            else
            {
                mainPage = _mapper.Map<EventStreetcode>(request.StreetcodeMainPage);
            }

            mainPage.CreatedAt = DateTime.Now;

            // Create Animation and Picture images

            if (mainPage.Images is not null && mainPage.Images.Any())
            {
                for (int i = 0; i < mainPage.Images.Count; i++)
                {
                    mainPage.Images[i] = _imageService.ConfigureImage(request.StreetcodeMainPage.Images[i]);
                }
            }

            // Create Audio

            if (mainPage.Audio is not null)
            {
                mainPage.Audio = _audioService.ConfigureAudio(request.StreetcodeMainPage.Audio);
            }

            var createdMainPage = await _repository.StreetcodeRepository.CreateAsync(mainPage);

            if (createdMainPage is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "main page block");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "main page block");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<StreetcodeDto>(createdMainPage));
        }
    }
}
