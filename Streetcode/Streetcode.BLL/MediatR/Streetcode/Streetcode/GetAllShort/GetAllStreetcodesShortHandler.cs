using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllShort
{
    public class GetAllStreetcodesShortHandler : IRequestHandler<GetAllStreetcodesShortQuery,
        Result<IEnumerable<StreetcodeShortDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllStreetcodesShortHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<StreetcodeShortDto>>> Handle(GetAllStreetcodesShortQuery request, CancellationToken cancellationToken)
        {
            var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllAsync();
            if (streetcodes != null)
            {
                return Result.Ok(_mapper.Map<IEnumerable<StreetcodeShortDto>>(streetcodes));
            }

            string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "streetcode");
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
