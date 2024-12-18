using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Media.Art;
using AutoMapper;
using Streetcode.BLL.Interfaces.Logging;
using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;
using StreetcodeArtEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeArt;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public class CreateArtHandler : IRequestHandler<CreateArtCommand, Result<ArtDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateArtHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<ArtDTO>> Handle(CreateArtCommand command, CancellationToken cancellationToken)
        {
            // Mapping ArtCreateDTO -> ArtEntity
            var newArt = _mapper.Map<ArtEntity>(command.newArt);
            if (newArt == null)
            {
                string errorMsg = "Cannot convert null to art";
                _logger.LogError(command, errorMsg);
                return Result.Fail(errorMsg);
            }

            // Check for image uniqueness
            var artsWithImage = await _repositoryWrapper.ArtRepository.GetAllAsync(
                predicate: art => art.ImageId == command.newArt.ImageId, null);

            if (artsWithImage.Any())
            {
                string errorMsg = "The Art with this image already exists. Please enter another image ID";
                _logger.LogError(command, errorMsg);
                return Result.Fail(errorMsg);
            }

            // Check for existence of StreetcodeIds if specified
            if (command.newArt.StreetcodeIds != null && command.newArt.StreetcodeIds.Any())
            {
                var foundStreetcodes = await _repositoryWrapper.StreetcodeRepository
                    .GetAllAsync(sc => command.newArt.StreetcodeIds.Contains(sc.Id));

                if (foundStreetcodes.Count() != command.newArt.StreetcodeIds.Count)
                {
                    var missingIds = command.newArt.StreetcodeIds.Except(foundStreetcodes.Select(sc => sc.Id));
                    string errorMsg = $"One or more Streetcode IDs do not exist. Missing IDs: {string.Join(", ", missingIds)}";
                    _logger.LogError(command, errorMsg);
                    return Result.Fail(errorMsg);
                }
            }

            var entity = await _repositoryWrapper.ArtRepository.CreateAsync(newArt);
            var isArtSaved = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!isArtSaved)
            {
                const string errorMsg = "Failed to create a Art";
                _logger.LogError(command, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            // Creating StreetcodeArt
            if (command.newArt.StreetcodeIds != null)
            {
                var streetcodeArts = command.newArt.StreetcodeIds
                    .Select(streetcodeId => new StreetcodeArtEntity
                    {
                        StreetcodeId = streetcodeId,
                        ArtId = entity.Id,
                    })
                    .ToList();

                await _repositoryWrapper.StreetcodeArtRepository.CreateRangeAsync(streetcodeArts);

                var areStreetcodeArtsSaved = await _repositoryWrapper.SaveChangesAsync() > 0;

                if(!areStreetcodeArtsSaved)
                {
                    const string errorMsg = "Failed to create StreetcodeArt records";
                    _logger.LogError(command, errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }
            }

            return Result.Ok(_mapper.Map<ArtDTO>(entity));
        }
    }
}
