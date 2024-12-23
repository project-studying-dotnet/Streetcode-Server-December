using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update
{
    public class UpdateRelatedTermHandler : IRequestHandler<UpdateRelatedTermCommand, Result<RelatedTermDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public UpdateRelatedTermHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<RelatedTermDTO>> Handle(UpdateRelatedTermCommand request, CancellationToken cancellationToken)
        {
            var existingRelatedTerm = await _repository.RelatedTermRepository
                .GetFirstOrDefaultBySpecAsync(new RelatedTermWithTermSpecification(request.RelatedTerm.Id));

            if (existingRelatedTerm == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "related term");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _mapper.Map(request.RelatedTerm, existingRelatedTerm);

            _repository.RelatedTermRepository.Update(existingRelatedTerm);

            var isSuccess = await _repository.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailUpdateError", "related term");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var updatedRelatedTermDTO = _mapper.Map<RelatedTermDTO>(existingRelatedTerm);

            return Result.Ok(updatedRelatedTermDTO);
        }
    }
}
