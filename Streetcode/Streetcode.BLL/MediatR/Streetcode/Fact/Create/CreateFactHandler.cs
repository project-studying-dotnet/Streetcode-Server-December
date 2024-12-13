using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create
{
    public class CreateFactHandler : IRequestHandler<CreateFactCommand, Result<FactDto>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateFactHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<FactDto>> Handle(CreateFactCommand command, CancellationToken cancellationToken)
        {
            var fact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(command.Fact);

            if (fact is null)
            {
                const string errorMsg = "Cannot create new fact!";
                _logger.LogError(command, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var createdFact = _repository.FactRepository.CreateAsync(fact);

            if (createdFact is null)
            {
                const string errorMsg = "Cannot create fact!";
                _logger.LogError(command, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                const string errorMsg = "Cannot save changes in the database!";
                _logger.LogError(command, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<FactDto>(createdFact));
        }
    }
}
