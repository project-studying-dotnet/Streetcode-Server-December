using EmailService.BLL.DTO;
using EmailService.BLL.Mediatr.Email;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.WebApi.Controllers
{
    public class EmailController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] EmailDto email)
        {
            return HandleResult(await Mediator.Send(new SendEmailCommand(email)));
        }

        [HttpPost("send-with-verification")]
        public async Task<IActionResult> SendWithVerification([FromBody] string email)
        {

            return HandleResult(await Mediator.Send(new SendEmailWithVerificationCommand(email)));
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            return HandleResult(await Mediator.Send(new ConfirmEmailCommand(email, token)));

        }
    }
}
