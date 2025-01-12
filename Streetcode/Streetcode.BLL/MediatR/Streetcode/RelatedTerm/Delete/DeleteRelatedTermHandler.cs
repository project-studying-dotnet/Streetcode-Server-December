using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Streetcode.RelatedTerm;
using Streetcode.BLL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete
{
    public class DeleteRelatedTermHandler : IRequestHandler<DeleteRelatedTermCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repository;

        public DeleteRelatedTermHandler(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<Result<Unit>> Handle(DeleteRelatedTermCommand request, CancellationToken cancellationToken)
        {
            var relatedTerm = await _repository.RelatedTermRepository
                .GetFirstOrDefaultBySpecAsync(new RelatedTermByWordAndTermIdSpecification(request.Word, request.TermId));

            if (relatedTerm is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "word", request.TermId);
                throw new Exception(errorMsg);
            }

            _repository.RelatedTermRepository.Delete(relatedTerm);

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

            if(!resultIsSuccess)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "word", request.TermId);
                throw new Exception(errorMsg);
            }

            return Result.Ok(Unit.Value);
        }
    }
}
