using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Audio;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streetcode.BLL.Specifications.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.UpdateMainPage
{
    public record UpdateStreetcodeMainPageHandler : IRequestHandler<UpdateStreetcodeMainPageCommand, Result<StreetcodeDto>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IImageService _imageService;
        private readonly IAudioService _audioService;

        public UpdateStreetcodeMainPageHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger, IImageService imageService, IAudioService audioService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
            _audioService = audioService;
        }

        public async Task<Result<StreetcodeDto>> Handle(UpdateStreetcodeMainPageCommand request, CancellationToken cancellationToken)
        {
            var mainPage = await _repository.StreetcodeRepository.GetFirstOrDefaultBySpecAsync(new StreetcodeMainPageSpecification(request.StreetcodeMainPage.Id));
            if (mainPage == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "streetcodecontent");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var images = mainPage.Images.Select(x => x.BlobName);
            var audio = mainPage.Audio.BlobName;

            if (request.StreetcodeMainPage.StreetcodeType == DAL.Enums.StreetcodeType.Person)
            {
                mainPage = _mapper.Map<PersonStreetcode>(request.StreetcodeMainPage);
            }
            else
            {
                mainPage = _mapper.Map<EventStreetcode>(request.StreetcodeMainPage);
            }

            // Create Animation and Picture images

            if (mainPage.Images is not null && mainPage.Images.Count != 0)
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

            mainPage.UpdatedAt = DateTime.Now;

            _repository.StreetcodeRepository.Update(mainPage);

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailUpdateError", "main page block");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            // Delete previous Animation and Picture images

            if (images is not null && images.Any())
            {
                foreach (var image in images)
                {
                    _imageService.DeleteImage(image!);
                }
            }

            // Delete previous Audio

            if (audio is not null)
            {
                _audioService.DeleteAudio(audio);
            }

            return Result.Ok(_mapper.Map<StreetcodeDto>(mainPage));
        }
    }
}
