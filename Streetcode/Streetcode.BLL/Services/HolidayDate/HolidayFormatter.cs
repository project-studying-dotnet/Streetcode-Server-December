using Newtonsoft.Json;
using Streetcode.BLL.Interfaces.HolidayFormatter;
using Streetcode.DAL.Entities.AdditionalContent.HolidayDate;
using System.Text.Json;

namespace Streetcode.BLL.Services.HolidayFormatter
{
    public class HolidayFormatter : IHolidayFormatter
    {
        public string GeneratePrompt(int day, int month, List<string> holidays)
        {
            return $"Создай JSON где ключ 'date' содержит дату {day}.{month}, " +
                   $"а ключ 'holidays' содержит список этих праздников (ключ 'holidayName'): {string.Join(", ", holidays)}" +
                   "и описание праздника придумай сам с ключом 'holidayDescription', в описание добавь про историю праздника, описание минимум 7 предложений." +
                   "В ответе возвращай только JSON но не в виде кода а просто одной строкой без переходов.Описание на украинском языке";
        }

        public HolidayResponse ParseResponse(string jsonResponse)
        {
            var formattedResponse = JsonDocument.Parse(jsonResponse)
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                .Trim('"');

            var holidayResponse = JsonConvert.DeserializeObject<HolidayResponse>(formattedResponse);

            if (holidayResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize the response into a HolidayResponse object.");
            }
            else
            {
                return holidayResponse;
            }
        }
    }
}
