using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.RedisCache;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using Streetcode.DAL.Repositories.Interfaces.Base;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll
{
    public record GetAllRelatedTermsHandler : IRequestHandler<GetAllRelatedTermsQuery, Result<IEnumerable<RelatedTermDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;
        private readonly IRedisCacheService _redisCacheService;
        private string _cacheKey = "AllRelatedTerms";

        public GetAllRelatedTermsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger, IRedisCacheService redisCacheService)
        {
            _mapper = mapper;
            _repository = repositoryWrapper;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<Result<IEnumerable<RelatedTermDTO>>> Handle(GetAllRelatedTermsQuery request, CancellationToken cancellationToken)
        {
            var relatedTerms = new List<RelatedTermEntity>();
            var relatedTermsFromCache = await _redisCacheService.GetCachedDataAsync<List<RelatedTermEntity>>(_cacheKey);
            if (relatedTermsFromCache == null)
            {
                var relatedTermsfromDb = await _repository.RelatedTermRepository
                    .GetAllBySpecAsync(new RelatedTermWithTermSpecification());

                if (relatedTermsfromDb is null)
                {
                    const string errorMsg = "Cannot get words";
                    _logger.LogError(request, errorMsg);
                    return new Error(errorMsg);
                }

                relatedTerms = relatedTermsfromDb.ToList();
                await _redisCacheService.SetCachedDataAsync(_cacheKey, relatedTerms, 10);
                _logger.LogInformation("Cached RelatedTerms for 10 minutes");
            }
            else 
            {
                relatedTerms = relatedTermsFromCache;
            }

            var relatedTermsDTO = _mapper.Map<IEnumerable<RelatedTermDTO>>(relatedTerms);

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
