using HtmlAgilityPack;
using Streetcode.DAL.Entities.AdditionalContent.HolidayDate.Dictionaries;
using System.Net.Http;

namespace Streetcode.WebApi.Controllers.HolidayDate.Parsers
{
    public class HolidaySource1Parser
    {
        private readonly HttpClient _httpClient;
        public HolidaySource1Parser(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI_Client");
        }

        public async Task<List<string>> GetHolidaysAsync(int day, int month)
        {
            if(day < HolidayDateDictionaries.MonthesBorders[month].Item1 || day > HolidayDateDictionaries.MonthesBorders[month].Item2)
            {
                throw new Exception($"Input day must be in borders from 1 to {HolidayDateDictionaries.MonthesBorders[month].Item2}");
            }

            var url = $"https://daytoday.ua/{day}-{HolidayDateDictionaries.MonthesToParse[month]}";
            var response = await _httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var holidayNodes = htmlDoc.DocumentNode.SelectNodes("/html/body/main/section/div/article/div[2]/div[3]/div/div/div")
                .Where(node => !node.Descendants("div")
                .Any(child => child.GetAttributeValue("class", "")
                .Contains("dayto-v-spysku-novyy") && child.GetAttributeValue("id", "").StartsWith("dayto-")));

            if (holidayNodes == null)
            {
                return new List<string> { "Свят не знайдено." };
            }

            var listHolidays = new List<string>();

            foreach (var node in holidayNodes)
            {
                var innerDiv = node.SelectSingleNode(".//div/h3");
                if (innerDiv != null)
                {
                    listHolidays.Add(innerDiv.InnerText.Trim());
                }
            }

            return listHolidays;
        }
    }
}
