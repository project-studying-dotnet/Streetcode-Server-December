using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Media.Audio;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Audio.GetById
{
    public class GetAudioByIdHandler : IRequestHandler<GetAudioByIdQuery, Result<AudioDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public GetAudioByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<AudioDto>> Handle(GetAudioByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var audio = await _repositoryWrapper.AudioRepository.GetFirstOrDefaultBySpecAsync(new GetAudioByIdSpecification(request.Id));

                if (audio is null)
                {
                    string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "audio", request.Id);
                    _logger.LogError(request, errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }

                var audioDto = _mapper.Map<AudioDto>(audio);

                audioDto.Base64 = await _blobService.FindFileInStorageAsBase64(audioDto.BlobName);

                return Result.Ok(audioDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(request, ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
        }
    }
}