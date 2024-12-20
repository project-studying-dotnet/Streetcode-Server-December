using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Specifications.Streetcode.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create
{
    public class CreateTextHandler : IRequestHandler<CreateTextCommand, Result<TextDTO>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        public CreateTextHandler(IRepositoryWrapper resitoryWrapper, IMapper mapper, ILoggerService loggerService)
        {
            _logger = loggerService;
            _mapper = mapper;
            _repository = resitoryWrapper;
        }

        public async Task<Result<TextDTO>> Handle(CreateTextCommand request, CancellationToken cancellationToken)
        {
            var streetcodeExists = await _repository.StreetcodeRepository.GetFirstOrDefaultBySpecAsync(new GetByStreetcodeIdSpecification(request.TextCreateDto.StreetcodeId));
            if (streetcodeExists == null)
            {
                const string errorMessage = "Streetcode does not exist.";
                _logger.LogError(request, errorMessage);
                throw new EntityNotFoundException();
            }

            var newText = _mapper.Map<DAL.Entities.Streetcode.TextContent.Text>(request.TextCreateDto);
            if (newText == null)
            {
                const string errorMessage = "Failed to map our DTO to TextDTO.";
                _logger.LogError(request, errorMessage);
                throw new Exception(errorMessage);
            }

            var createdText = await _repository.TextRepository.CreateAsync(newText);
            var isSucess = await _repository.SaveChangesAsync();
            if (isSucess == 0)
            {
                const string errorMessage = "Failed to save changes to the database.";
                _logger.LogError(request, errorMessage);
                throw new Exception(errorMessage);
            }

            return Result.Ok(_mapper.Map<TextDTO>(createdText));
        }
    }
}
