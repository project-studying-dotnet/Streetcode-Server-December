using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId
{

    public class GetToponymsByStreetcodeIdHandler : IRequestHandler<GetToponymsByStreetcodeIdQuery, Result<IEnumerable<ToponymDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetToponymsByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<ToponymDto>>> Handle(GetToponymsByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var toponyms = await _repositoryWrapper
                .ToponymRepository
                .GetAllAsync(
                    predicate: sc => sc.Streetcodes.Any(s => s.Id == request.StreetcodeId),
                    include: scl => scl
                        .Include(sc => sc.Coordinate));

            if (toponyms is null || !toponyms.Any())
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "toponym", request.StreetcodeId);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var filteredToponyms = toponyms.DistinctBy(x => x.StreetName);
            var toponymDto = filteredToponyms.GroupBy(x => x.StreetName).Select(group => group.First()).Select(x => _mapper.Map<ToponymDto>(x));
            return Result.Ok(toponymDto);
        }
    }
}