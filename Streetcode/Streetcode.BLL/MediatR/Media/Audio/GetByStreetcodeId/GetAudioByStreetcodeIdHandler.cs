using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.MediatR.ResultVariations;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId
{

    public class GetAudioByStreetcodeIdHandler : IRequestHandler<GetAudioByStreetcodeIdQuery, Result<AudioDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public GetAudioByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<AudioDto>> Handle(GetAudioByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var streetcode = await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(
                s => s.Id == request.StreetcodeId,
                include: q => q.Include(s => s.Audio)!);

            if (streetcode == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "audio", request.StreetcodeId);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            NullResult<AudioDto> result = new NullResult<AudioDto>();

            if (streetcode.Audio != null)
            {
                AudioDto audioDto = _mapper.Map<AudioDto>(streetcode.Audio);
                audioDto.Base64 = _blobService.FindFileInStorageAsBase64(audioDto.BlobName);
                result.WithValue(audioDto);
            }

            return result;
        }
    }
}