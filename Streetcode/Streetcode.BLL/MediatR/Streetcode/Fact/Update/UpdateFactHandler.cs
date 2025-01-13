using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update
{
    public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<FactDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<FactDto>> Handle(UpdateFactCommand request, CancellationToken cancellationToken)
        {
            // Check if the fact exists
            var existingFact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(f => f.Id == request.Fact.Id);

            if (existingFact == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "fact", request.Fact.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            try
            {
                // Map updated properties to the existing entity
                _mapper.Map(request.Fact, existingFact);

                // Update the entity in the database
                _repositoryWrapper.FactRepository.Update(existingFact);
                await _repositoryWrapper.SaveChangesAsync();

                // Map back to DTO for the result
                return Result.Ok(_mapper.Map<FactDto>(existingFact));
            }
            catch (Exception ex)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailUpdateError", "fact");
                _logger.LogError(ex, errorMsg);
                return Result.Fail(new Error(errorMsg).CausedBy(ex.Message));
            }
        }
    }
}
