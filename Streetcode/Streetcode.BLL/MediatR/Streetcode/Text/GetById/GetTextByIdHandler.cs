using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Text.GetById
{
    public class GetTextByIdHandler : IRequestHandler<GetTextByIdQuery, Result<TextDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetTextByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TextDto>> Handle(GetTextByIdQuery request, CancellationToken cancellationToken)
        {
            var text = await _repositoryWrapper.TextRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);

            if (text is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "text", request.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<TextDto>(text));
        }
    }
}