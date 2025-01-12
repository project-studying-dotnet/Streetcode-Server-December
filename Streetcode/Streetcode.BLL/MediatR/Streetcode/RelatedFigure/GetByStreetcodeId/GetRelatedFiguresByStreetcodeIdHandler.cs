using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId
{
    public class GetRelatedFiguresByStreetcodeIdHandler : IRequestHandler<GetRelatedFigureByStreetcodeIdQuery, Result<IEnumerable<RelatedFigureDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetRelatedFiguresByStreetcodeIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<RelatedFigureDto>>> Handle(GetRelatedFigureByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var relatedFigureIds = GetRelatedFigureIdsByStreetcodeId(request.StreetcodeId);

            if (relatedFigureIds is null)
            {
                string errorMsg = $"Cannot find any related figures by a streetcode id: {request.StreetcodeId}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var relatedFigures = await _repositoryWrapper.StreetcodeRepository.GetAllAsync(
              predicate: sc => relatedFigureIds.Any(id => id == sc.Id) && sc.Status == Streetcode.Domain.Enums.StreetcodeStatus.Published,
              include: scl => scl.Include(sc => sc.Images).ThenInclude(img => img.ImageDetails)
                                 .Include(sc => sc.Tags));

            if (relatedFigures is null)
            {
                string errorMsg = $"Cannot find any related figures by a streetcode id: {request.StreetcodeId}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            foreach (StreetcodeContent streetcode in relatedFigures)
            {
                if (streetcode.Images != null)
                {
                    streetcode.Images = streetcode.Images.OrderBy(img => img.ImageDetails?.Alt).ToList();
                }
            }

            return Result.Ok(_mapper.Map<IEnumerable<RelatedFigureDto>>(relatedFigures));
        }

        private IQueryable<int> GetRelatedFigureIdsByStreetcodeId(int StreetcodeId)
        {
            try
            {
                var observerIds = _repositoryWrapper.RelatedFigureRepository
                .FindAll(f => f.TargetId == StreetcodeId).Select(o => o.ObserverId);

                var targetIds = _repositoryWrapper.RelatedFigureRepository
                    .FindAll(f => f.ObserverId == StreetcodeId).Select(t => t.TargetId);

                return observerIds.Union(targetIds).Distinct();
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
    }
}