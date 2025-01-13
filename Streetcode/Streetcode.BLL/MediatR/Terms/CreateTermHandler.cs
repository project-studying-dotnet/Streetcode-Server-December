using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Terms
{
    public class CreateTermHandler : IRequestHandler<CreateTermCommand, Result<TermDto>>
    {
        private readonly ITermRepository _termRepository;
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper; 
        private readonly ILoggerService _logger;

        public CreateTermHandler(ITermRepository termRepository, IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _termRepository = termRepository;
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<TermDto>> Handle(CreateTermCommand request, CancellationToken cancellationToken)
        {

            var term = _mapper.Map<Term>(request.TermCreateDTO);

            _termRepository.Create(term);

            var saveResult = await _repositoryWrapper.SaveChangesAsync();


            if (saveResult == 0)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "term");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var termDTO = _mapper.Map<TermDto>(term);
            return Result.Ok(termDTO);
        }
    }
}