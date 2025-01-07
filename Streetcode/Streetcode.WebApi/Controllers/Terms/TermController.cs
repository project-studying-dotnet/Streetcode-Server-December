using MediatR;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.DTO.Terms;
using Streetcode.BLL.MediatR.Terms;

namespace Streetcode.WebApi.Controllers.Terms
{
    [ApiController]
    [Route("api/[controller]")]
    public class TermController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TermController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTerm([FromBody] TermCreateDTO termCreateDTO)
        {
            var command = new CreateTermCommand(termCreateDTO);
            var result = await _mediator.Send(command);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors); 
            }

            return CreatedAtAction(nameof(GetTerm), new { id = result.Value.Id }, result.Value); 
        }
 
        [HttpGet("{id}")]
        public IActionResult GetTerm(int id)
        {
            return Ok(); 
        }
    }
}
