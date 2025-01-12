using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Streetcode.Term.GetById
{
    public class GetTermByIdHandler : IRequestHandler<GetTermByIdQuery, Result<TermDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetTermByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TermDto>> Handle(GetTermByIdQuery request, CancellationToken cancellationToken)
        {
            var term = await _repositoryWrapper.TermRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);

            if (term is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "term", request.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<TermDto>(term));
        }
    }
}