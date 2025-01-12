using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.Domain.Entities.News;

namespace Streetcode.BLL.MediatR.Newss.Update
{
    public class UpdateNewsHandler : IRequestHandler<UpdateNewsCommand, Result<NewsDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly IBlobService _blobSevice;
        private readonly ILoggerService _logger;
        public UpdateNewsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobSevice = blobService;
            _logger = logger;
        }

        public async Task<Result<NewsDto>> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            var news = _mapper.Map<News>(request.news);
            if (news is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("ConvertationError", "null", "news");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var response = _mapper.Map<NewsDto>(news);

            if (news.Image is not null)
            {
                response.Image.Base64 = await _blobSevice.FindFileInStorageAsBase64(response.Image.BlobName);
            }
            else
            {
                var img = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(x => x.Id == response.ImageId);
                if (img != null)
                {
                    _repositoryWrapper.ImageRepository.Delete(img);
                }
            }

            _repositoryWrapper.NewsRepository.Update(news);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if(resultIsSuccess)
            {
                return Result.Ok(response);
            }
            else
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailUpdateError", "news");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
