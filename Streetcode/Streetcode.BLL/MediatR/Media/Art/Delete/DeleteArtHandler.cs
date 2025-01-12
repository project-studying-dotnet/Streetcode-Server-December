using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Art.Delete
{
    public class DeleteArtHandler : IRequestHandler<DeleteArtCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteArtHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteArtCommand request, CancellationToken cancellationToken)
        {
            var art = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(
                f => f.Id == request.Id, i => i.Include(a => a.Image));

            if (art is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "art", request.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.ArtRepository.Delete(art);

            if (art.Image != null)
            {
                _repositoryWrapper.ImageRepository.Delete(art.Image);
            }

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "art");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(Unit.Value);
        }
    }
}
