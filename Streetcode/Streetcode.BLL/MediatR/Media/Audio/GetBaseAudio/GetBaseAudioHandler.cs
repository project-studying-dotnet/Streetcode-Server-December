﻿using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Media.Audio;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio
{
    public class GetBaseAudioHandler : IRequestHandler<GetBaseAudioQuery, Result<MemoryStream>>
    {
        private readonly IBlobService _blobStorage;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetBaseAudioHandler(IBlobService blobService, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _blobStorage = blobService;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<MemoryStream>> Handle(GetBaseAudioQuery request, CancellationToken cancellationToken)
        {
            var audio = await _repositoryWrapper.AudioRepository.GetFirstOrDefaultBySpecAsync(new GetAudioByIdSpecification(request.Id));

            if (audio is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "audio", request.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return await _blobStorage.FindFileInStorageAsMemoryStream(audio.BlobName);
        }
    }
}
