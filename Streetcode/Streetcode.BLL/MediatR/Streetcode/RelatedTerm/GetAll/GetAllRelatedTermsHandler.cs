using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using Streetcode.DAL.Caching.RedisCache;
using Streetcode.DAL.Repositories.Interfaces.Base;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll
{
    public record GetAllRelatedTermsHandler : IRequestHandler<GetAllRelatedTermsQuery, Result<IEnumerable<RelatedTermDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public GetAllRelatedTermsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<RelatedTermDto>>> Handle(GetAllRelatedTermsQuery request, CancellationToken cancellationToken)
        {
            var relatedTerms = await _repository.RelatedTermRepository
                .GetAllBySpecAsync(new RelatedTermWithTermSpecification());

            if (relatedTerms is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "words");
                _logger.LogError(request, errorMsg);
                return new Error(errorMsg);
            }

            var relatedTermsDTO = _mapper.Map<IEnumerable<RelatedTermDto>>(relatedTerms);

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
