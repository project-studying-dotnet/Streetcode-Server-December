using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId
{
    public class GetCoordinatesByStreetcodeIdHandler : IRequestHandler<GetCoordinatesByStreetcodeIdQuery, Result<IEnumerable<StreetcodeCoordinateDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetCoordinatesByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<StreetcodeCoordinateDto>>> Handle(GetCoordinatesByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            if ((await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(s => s.Id == request.StreetcodeId)) is null)
            {
                return Result.Fail(
                    new Error(ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "coordinate", request.StreetcodeId)));
            }

            var coordinates = await _repositoryWrapper.StreetcodeCoordinateRepository
                    .GetAllAsync(c => c.StreetcodeId == request.StreetcodeId);

            if (coordinates is null)
            {
                var msg = ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "coordinate", request.StreetcodeId);
                _logger.LogError(request, msg);
                return Result.Fail(new Error(msg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<StreetcodeCoordinateDto>>(coordinates));
        }
    }
}