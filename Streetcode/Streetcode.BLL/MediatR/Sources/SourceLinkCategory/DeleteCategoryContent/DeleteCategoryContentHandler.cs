using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent
{
	public class DeleteCategoryContentHandler : IRequestHandler<DeleteCategoryContentCommand, Result<StreetcodeCategoryContentDTO>>
	{
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly IMapper _mapper;
		private readonly ILoggerService _logger;

		public DeleteCategoryContentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
		{
			_repositoryWrapper = repositoryWrapper;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Result<StreetcodeCategoryContentDTO>> Handle(DeleteCategoryContentCommand request, CancellationToken cancellationToken)
		{
			var source = await _repositoryWrapper.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(s => s.Id == request.id);
			if (source == null)
			{
				string errMsg = "No source with such id";
				_logger.LogError(request, errMsg);
				return Result.Fail(errMsg);
			}
			else
			{
				_repositoryWrapper.StreetcodeCategoryContentRepository.Delete(source);
				try
				{
					_repositoryWrapper.SaveChanges();
					return Result.Ok(_mapper.Map<StreetcodeCategoryContentDTO>(source));
				}
				catch (Exception ex)
				{
					_logger.LogError(request, ex.Message);
					return Result.Fail(ex.Message);
				}
			}
		}
	}
}
