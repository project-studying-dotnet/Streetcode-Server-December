using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Audio.Create
{
    public class CreateAudioHandler : IRequestHandler<CreateAudioCommand, Result<AudioDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public CreateAudioHandler(
            IBlobService blobService,
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILoggerService logger)
        {
            _blobService = blobService;
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<AudioDto>> Handle(CreateAudioCommand request, CancellationToken cancellationToken)
        {
            string hashBlobStorageName = await _blobService.SaveFileInStorage(
                request.Audio.BaseFormat,
                request.Audio.Title,
                request.Audio.Extension);

            var audio = _mapper.Map<Domain.Entities.Media.Audio>(request.Audio);

            audio.BlobName = $"{hashBlobStorageName}.{request.Audio.Extension}";

            await _repositoryWrapper.AudioRepository.CreateAsync(audio);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            var createdAudio = _mapper.Map<AudioDto>(audio);

            if (resultIsSuccess)
            {
                return Result.Ok(createdAudio);
            }
            else
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "audio");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
