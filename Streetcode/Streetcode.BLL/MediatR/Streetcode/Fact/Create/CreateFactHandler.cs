using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create
{
    public class CreateFactHandler : IRequestHandler<CreateFactCommand, Result<FactDto>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IImageService _imageService;

        public CreateFactHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger, IImageService imageService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
        }

        public async Task<Result<FactDto>> Handle(CreateFactCommand command, CancellationToken cancellationToken)
        {
            var fact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(command.Fact);

            if (fact is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "fact");
                _logger.LogError(command, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            fact.Index = (await _repository.FactRepository.GetAllAsync(g => g.StreetcodeId == fact.StreetcodeId))
                .Select(s => s.Index).Max() + 1;

            if (fact.Image is null)
            {
                const string errorMsg = "Cannot create an image!";
                _logger.LogError(command, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            fact.Image = _imageService.ConfigureImage(command.Fact.Image);

            var createdFact = await _repository.FactRepository.CreateAsync(fact);

            if (createdFact is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "fact");
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
