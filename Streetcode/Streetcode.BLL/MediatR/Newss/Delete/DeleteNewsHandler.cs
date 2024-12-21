using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Newss.Delete
{
    public class DeleteNewsHandler : IRequestHandler<DeleteNewsCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        public DeleteNewsHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
        {
            int id = request.id;
            var news = await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "news", request.id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            if (news.Image is not null)
            {
                _repositoryWrapper.ImageRepository.Delete(news.Image);
            }

            _repositoryWrapper.NewsRepository.Delete(news);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
            if(resultIsSuccess)
            {
                return Result.Ok(Unit.Value);
            }
            else
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantDeleteError", "news", request.id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
