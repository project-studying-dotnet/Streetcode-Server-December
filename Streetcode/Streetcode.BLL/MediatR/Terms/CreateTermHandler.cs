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

namespace Streetcode.BLL.MediatR.Terms
{
    public class CreateTermHandler : IRequestHandler<CreateTermCommand, Result<TermDto>>
    {
        private readonly ITermRepository _termRepository;
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper; 

        public CreateTermHandler(ITermRepository termRepository, IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _termRepository = termRepository;
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<TermDto>> Handle(CreateTermCommand request, CancellationToken cancellationToken)
        {
            var term = _mapper.Map<Term>(request.TermCreateDTO);

            _termRepository.Create(term);

            var saveResult = await _repositoryWrapper.SaveChangesAsync();

            if (saveResult == 0)
            {
                return Result.Fail<TermDto>("Failed to save the term.");
            }

            var termDTO = _mapper.Map<TermDto>(term);

            return Result.Ok(termDTO);
        }
    }
}
