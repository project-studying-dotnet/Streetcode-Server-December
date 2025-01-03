using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Services.HolidayDate;
using Streetcode.WebApi.Controllers.HolidayDate.Parsers;

namespace Streetcode.WebApi.Controllers.HolidayDate
{
    public class HolidayDateController : BaseApiController
    {
        private readonly HolidaySource1Parser _holidayParser;
        private readonly HolidayDateService _holidayService;

        public HolidayDateController(HolidaySource1Parser holidayParser, HolidayDateService holidayService)
        {
            _holidayParser = holidayParser;
            _holidayService = holidayService;
        }


        [HttpGet("get-holidays-chatGpt")]
        public async Task<IActionResult> GetHolidays(int day, int month)
        {
            try
            {
                var holidays = await _holidayParser.GetHolidaysAsync(day, month);

                var formattedJson = await _holidayService.GetFormattedHolidaysAsync(day, month, holidays);
                return Ok(new { Data = formattedJson });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
