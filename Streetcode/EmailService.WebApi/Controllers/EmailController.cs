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
    }
}
