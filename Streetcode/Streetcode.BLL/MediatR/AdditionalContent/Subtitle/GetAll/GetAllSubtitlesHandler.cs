using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetAll
{
    public class GetAllSubtitlesHandler : IRequestHandler<GetAllSubtitlesQuery, Result<IEnumerable<SubtitleDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllSubtitlesHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<SubtitleDto>>> Handle(GetAllSubtitlesQuery request, CancellationToken cancellationToken)
        {
            var subtitles = await _repositoryWrapper.SubtitleRepository.GetAllAsync();

            if (subtitles is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "subtitles");

                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<SubtitleDto>>(subtitles));
        }
    }
}