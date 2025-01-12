using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create
{
    public class CreateCoordinateHandler : IRequestHandler<CreateCoordinateCommand, Result<Unit>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public CreateCoordinateHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(CreateCoordinateCommand request, CancellationToken cancellationToken)
        {
            var streetcodeCoordinate = _mapper.Map<Streetcode.Domain.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(request.StreetcodeCoordinate);

            if (streetcodeCoordinate is null)
            {
                return Result.Fail(new Error(ErrorManager.GetCustomErrorText("ConvertationError", "null", "streetcodeCoordinate")));
            }

            await _repositoryWrapper.StreetcodeCoordinateRepository.CreateAsync(streetcodeCoordinate);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
            return resultIsSuccess ? Result.Ok(Unit.Value) : Result.Fail(new Error(ErrorManager.GetCustomErrorText("FailCreateError", "streetcodeCoordinate")));
        }
    }
}