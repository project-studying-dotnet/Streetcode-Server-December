﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Audio.GetAll
{
    public class GetAllAudiosHandler : IRequestHandler<GetAllAudiosQuery, Result<IEnumerable<AudioDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public GetAllAudiosHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<AudioDto>>> Handle(GetAllAudiosQuery request, CancellationToken cancellationToken)
        {
            var audios = await _repositoryWrapper.AudioRepository.GetAllBySpecAsync();

            if (audios is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "audio");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var audioDtos = _mapper.Map<IEnumerable<AudioDto>>(audios);
            foreach (var audio in audioDtos)
            {
                audio.Base64 = await _blobService.FindFileInStorageAsBase64(audio.BlobName);
            }

            return Result.Ok(audioDtos);
        }
    }
}