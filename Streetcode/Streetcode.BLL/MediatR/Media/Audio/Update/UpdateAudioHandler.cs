using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Audio.Update
{
    public class UpdateAudioHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public UpdateAudioHandler(IMapper mapper, IRepositoryWrapper wrapper, IBlobService blobService,ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = wrapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<AudioDto>> Handle(UpdateAudioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingAudio = await _repositoryWrapper.AudioRepository
                    .GetFirstOrDefaultAsync(a => a.Id == request.Audio.Id);

                if (existingAudio is null)
                {
                    string errorMsg = ErrorManager.GetCustomErrorText($"CannotFindAnAudioWithTheCorrespondingStreetcodeId {request.Audio.Id}");
                    _logger.LogError(request, errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }

                var updatedAudio = _mapper.Map<DAL.Entities.Media.Audio>(request.Audio);

                var newName = await _blobService.UpdateFileInStorage(
                    existingAudio.BlobName,
                    request.Audio.BaseFormat,
                    request.Audio.Title,
                    request.Audio.Extension);

                updatedAudio.BlobName = $"{newName}.{request.Audio.Extension}";

                _repositoryWrapper.AudioRepository.Update(updatedAudio);

                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                var createdAudio = _mapper.Map<AudioDto>(updatedAudio);

                createdAudio.Base64 = await _blobService.FindFileInStorageAsBase64(createdAudio.BlobName);

                if (resultIsSuccess)
                {
                    return Result.Ok(createdAudio);
                }
                else
                {
                    string errorMsg = ErrorManager.GetCustomErrorText("FailedToUpdateAudio");
                    _logger.LogError(request, errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                _logger.LogError(ex, $"Error in {nameof(UpdateAudioHandler)}: errorMsg");
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
