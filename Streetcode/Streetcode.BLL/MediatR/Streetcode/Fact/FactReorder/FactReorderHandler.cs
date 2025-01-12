using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Specifications.Streetcode.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder
{
    public class FactReorderHandler : IRequestHandler<FactReorderCommand, Result<List<FactDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public FactReorderHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<List<FactDto>>> Handle(FactReorderCommand request, CancellationToken cancellationToken)
        {
            var existingFacts = await _repository.FactRepository
                .GetAllBySpecAsync(new GetAllFactsByStreetcodeIdSpecification(request.FactReorderDto.StreetcodeId));

            if (existingFacts == null || !existingFacts.Any())
            {
                const string errorMsg = "Facts not found!";
                _logger.LogError(request, errorMsg);
                throw new EntityNotFoundException();
            }

            if (request.FactReorderDto.IdPositions.Count() != request.FactReorderDto.IdPositions.Distinct().Count())
            {
                const string errorMsg = "IdPositions should not have duplicates";
                _logger.LogError(request, errorMsg);
                throw new Exception(errorMsg);
            }

            if (existingFacts.Count() != request.FactReorderDto.IdPositions.Count || !request.FactReorderDto.IdPositions.All(id => existingFacts.Any(x => x.Id == id)))
            {
                const string errorMsg = "All IdPositions should be related to Facts with provided StreetcodeId";
                _logger.LogError(request, errorMsg);
                throw new Exception(errorMsg);
            }

            foreach (var fact in existingFacts)
            {
                fact.Index = request.FactReorderDto.IdPositions.IndexOf(fact.Id) + 1;
            }

            var isSuccess = await _repository.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                const string errorMsg = "Failed to save the reordered facts.";
                _logger.LogError(request, errorMsg);
                throw new Exception(errorMsg);
            }

            return Result.Ok(_mapper.Map<List<FactDto>>(existingFacts));
        }
    }
}
