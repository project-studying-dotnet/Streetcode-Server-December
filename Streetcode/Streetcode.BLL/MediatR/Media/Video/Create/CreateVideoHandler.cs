using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Base;

namespace Streetcode.BLL.MediatR.Media.Video.Create
{
    public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<VideoDTO>>
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateVideoHandler(IRepositoryWrapper repoWrapper, IMapper mapper, ILoggerService logger)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VideoDTO>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
        {
            var isStreetcodeExsit = await _repoWrapper.StreetcodeRepository.
                GetFirstOrDefaultAsync(s => s.Id == request.Video.StreetcodeId);

            if (isStreetcodeExsit == null)
            {
                string error = $"Cant find StreetcodeContent with id={request.Video.StreetcodeId} for video creation";
                _logger.LogError(request, error);
                return Result.Fail(new Error(error));
            }

            var videoEntity = _mapper.Map<DAL.Entities.Media.Video>(request.Video);
            videoEntity.Title = $"NewVideo";
            videoEntity.Description = $"{DateTime.Now} was added";

            await _repoWrapper.VideoRepository.CreateAsync(videoEntity);

            var res = await _repoWrapper.SaveChangesAsync() > 0;

            var videoDTO = _mapper.Map<VideoDTO>(videoEntity);

            if (res)
            {
                return Result.Ok(videoDTO);
            }
            else
            {
                const string errorMessage = "Failed to create an video";
                _logger.LogError(request, errorMessage);
                return Result.Fail(new Error(errorMessage));
            }
        }
    }
}
