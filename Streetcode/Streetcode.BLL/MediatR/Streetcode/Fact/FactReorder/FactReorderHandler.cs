using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder
{
    public class FactReorderHandler : IRequestHandler<FactReorderCommand, Result<FactReorderDto>>
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

        public async Task<Result<FactReorderDto>> Handle(FactReorderCommand request, CancellationToken cancellationToken)
        {
            if (request.FactReorderDto.IdPositions.Count != request.FactReorderDto.FactReorders.Count)
            {
                const string errorMsg = "FactReorders and Positions should have equal count of objects!";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var factIds = request.FactReorderDto.FactReorders
                .Select(f => f.Id).Order().ToList();

            var existingFacts = await _repository.FactRepository
                .GetAllAsync(rt => factIds.Contains(rt.Id) && request.FactReorderDto.IdPositions.Contains(rt.Id));

            if (existingFacts == null || !existingFacts.Any())
            {
                const string errorMsg = "Facts not found!";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            if (existingFacts.Count() != factIds.Count)
            {
                const string errorMsg = "All Facts should exist";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            int streetcodeId = existingFacts.First().Id;
            if (existingFacts.Count() != existingFacts.Where(x => x.StreetcodeId == streetcodeId).Count())
            {
                const string errorMsg = "All Facts should be linked to one Streetcode";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var reorderedFacts = new List<FactUpdateCreateDto>();
            for (int i = 0; i < request.FactReorderDto.IdPositions.Count; i++)
            {
                var factToReorder = request.FactReorderDto.FactReorders.Where(x => x.Id == request.FactReorderDto.IdPositions[i]).First();
                var reorderedFact = new FactUpdateCreateDto()
                {
                    Id = factIds[i],
                    Title = factToReorder.Title,
                    ImageId = factToReorder.ImageId,
                    FactContent = factToReorder.FactContent,
                    ImageDescription = factToReorder.ImageDescription,
                };
                reorderedFacts.Add(reorderedFact);
            }

            _mapper.Map(reorderedFacts, existingFacts);

            foreach (var fact in existingFacts)
            {
                fact.StreetcodeId = streetcodeId;
            }

            _repository.FactRepository.UpdateRange(existingFacts);

            var isSuccess = await _repository.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                const string errorMsg = "Failed to save the reordered facts.";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var updatedFactReorderDto = new FactReorderDto
            {
                FactReorders = _mapper.Map<List<FactUpdateCreateDto>>(reorderedFacts),
                IdPositions = [],
            };

            return Result.Ok(updatedFactReorderDto);
        }
    }
}
