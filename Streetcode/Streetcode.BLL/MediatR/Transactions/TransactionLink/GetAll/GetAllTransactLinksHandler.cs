using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Transactions.TransactionLink.GetAll
{
    public class GetAllTransactLinksHandler : IRequestHandler<GetAllTransactLinksQuery, Result<IEnumerable<TransactLinkDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllTransactLinksHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<TransactLinkDto>>> Handle(GetAllTransactLinksQuery request, CancellationToken cancellationToken)
        {
            var transactLinks = await _repositoryWrapper.TransactLinksRepository.GetAllAsync();

            if (transactLinks is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "transaction link");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<TransactLinkDto>>(transactLinks));
        }
    }
}
