using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById
{
    public record GetRelatedTermByIdHandler : IRequestHandler<GetRelatedTermByIdQuery, Result<RelatedTermDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public GetRelatedTermByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<RelatedTermDTO>> Handle(GetRelatedTermByIdQuery request, CancellationToken cancellationToken)
        {
            var relatedTerms = await _repository.RelatedTermRepository
                .GetFirstOrDefaultBySpecAsync(new RelatedTermWithTermSpecification(request.id));

            if (relatedTerms is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "word" , request.id);
                _logger.LogError(request, errorMsg);
                return new Error(errorMsg);
            }

            var relatedTermsDTO = _mapper.Map<RelatedTermDTO>(relatedTerms);

            if (relatedTermsDTO is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantCreateError", "DTOS for related words");
                _logger.LogError(request, errorMsg);
                return new Error(errorMsg);
            }

            return Result.Ok(relatedTermsDTO);
        }
    }
}
