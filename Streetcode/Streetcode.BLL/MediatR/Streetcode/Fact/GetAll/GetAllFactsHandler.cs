using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.GetAll
{
    public class GetAllFactsHandler : IRequestHandler<GetAllFactsQuery, Result<IEnumerable<FactDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllFactsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<FactDto>>> Handle(GetAllFactsQuery request, CancellationToken cancellationToken)
        {
            var facts = await _repositoryWrapper.FactRepository.GetAllAsync();

            if (facts is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "fact");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<FactDto>>(facts));
        }
    }
}