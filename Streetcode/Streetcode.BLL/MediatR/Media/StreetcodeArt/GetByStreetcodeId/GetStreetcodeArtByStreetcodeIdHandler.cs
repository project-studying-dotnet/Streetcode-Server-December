using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.StreetcodeArt.GetByStreetcodeId
{
    public class GetStreetcodeArtByStreetcodeIdHandler : IRequestHandler<GetStreetcodeArtByStreetcodeIdQuery, Result<IEnumerable<StreetcodeArtDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public GetStreetcodeArtByStreetcodeIdHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IBlobService blobService,
            ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<StreetcodeArtDto>>> Handle(GetStreetcodeArtByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            /*
            if ((await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(s => s.Id == request.StreetcodeId)) is null)
            {
                return Result.Fail(
                    new Error($"Cannot find a streetcode arts by a streetcode id: {request.StreetcodeId}, because such streetcode doesn`t exist"));
            }
            */

            var art = await _repositoryWrapper
            .StreetcodeArtRepository
            .GetAllAsync(
                predicate: s => s.StreetcodeId == request.StreetcodeId,
                include: art => art
                    .Include(a => a.Art)
                    .Include(i => i.Art.Image) !);

            if (art is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "art", request.StreetcodeId);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var artsDto = _mapper.Map<IEnumerable<StreetcodeArtDto>>(art);

            foreach (var artDto in artsDto)
            {
                artDto.Art.Image.Base64 = await _blobService.FindFileInStorageAsBase64(artDto.Art.Image.BlobName);
            }

            return Result.Ok(artsDto);
        }
    }
}