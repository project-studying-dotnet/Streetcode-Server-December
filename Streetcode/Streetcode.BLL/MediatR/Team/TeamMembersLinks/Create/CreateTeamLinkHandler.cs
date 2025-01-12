using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create
{
    public class CreateTeamLinkHandler : IRequestHandler<CreateTeamLinkCommand, Result<TeamMemberLinkDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public CreateTeamLinkHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TeamMemberLinkDto>> Handle(CreateTeamLinkCommand request, CancellationToken cancellationToken)
        {
            var teamMemberLink = _mapper.Map<Streetcode.Domain.Entities.Team.TeamMemberLink>(request.teamMember);

            if (teamMemberLink is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("ConvertationError", "null", "team link");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var createdTeamLink = _repository.TeamLinkRepository.Create(teamMemberLink);

            if (createdTeamLink is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "positions");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "team");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var createdTeamLinkDTO = _mapper.Map<TeamMemberLinkDto>(createdTeamLink);

            if(createdTeamLinkDTO != null)
            {
                return Result.Ok(createdTeamLinkDTO);
            }
            else
            {
                const string errorMsg = "Failed to map created team link";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
