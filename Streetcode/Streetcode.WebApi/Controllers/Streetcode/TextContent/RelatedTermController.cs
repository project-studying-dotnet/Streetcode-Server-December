using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent
{
    public class RelatedTermController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllRelatedTermsQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetRelatedTermByIdQuery(id)));
        }

        [HttpGet("{termid:int}")]
        public async Task<IActionResult> GetByTermId([FromRoute] int termid)
        {
            return HandleResult(await Mediator.Send(new GetAllRelatedTermsByTermIdQuery(termid)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RelatedTermDTO relatedTerm)
        {
            return HandleResult(await Mediator.Send(new CreateRelatedTermCommand(relatedTerm)));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RelatedTermDTO relatedTerm)
        {
            return HandleResult(await Mediator.Send(new UpdateRelatedTermCommand(relatedTerm)));
        }

        [HttpDelete("{word}/{termId:int}")]
        public async Task<IActionResult> Delete([FromRoute] string word, [FromRoute] int termId)
        {
            return HandleResult(await Mediator.Send(new DeleteRelatedTermCommand(word, termId)));
        }
    }
}
