using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Newss.GetById
{
    public class GetNewsByIdHandler : IRequestHandler<GetNewsByIdQuery, Result<NewsDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;
        public GetNewsByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<NewsDto>> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            int id = request.id;
            var newsDTO = _mapper.Map<NewsDto>(await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
                predicate: sc => sc.Id == id,
                include: scl => scl
                    .Include(sc => sc.Image)));
            if(newsDTO is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "news", request.id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            if (newsDTO.Image is not null)
            {
                newsDTO.Image.Base64 = await _blobService.FindFileInStorageAsBase64(newsDTO.Image.BlobName);
            }

            return Result.Ok(newsDTO);
        }
    }
}