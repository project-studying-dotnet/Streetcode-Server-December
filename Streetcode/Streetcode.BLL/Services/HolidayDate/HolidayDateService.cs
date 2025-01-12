using Streetcode.BLL.Interfaces.HolidayFormatter;
using Streetcode.BLL.Services.OpenAI;
using Streetcode.Domain.Entities.AdditionalContent.HolidayDate;

namespace Streetcode.BLL.Services.HolidayDate
{
    public class HolidayDateService
    {
        private readonly OpenAIService _openAiService;
        private readonly IHolidayFormatter _holidayFormatter;

        public HolidayDateService(OpenAIService chatGptService, IHolidayFormatter holidayFormatter)
        {
            _openAiService = chatGptService;
            _holidayFormatter = holidayFormatter;
        }

        public async Task<HolidayResponse> GetFormattedHolidaysAsync(int day, int month, List<string> holidays)
        {
            var prompt = _holidayFormatter.GeneratePrompt(day, month, holidays);
            var jsonResponse = await _openAiService.GetResponseAsync(prompt, holidays.Count * 500);
            return _holidayFormatter.ParseResponse(jsonResponse);
        }
    }
}
