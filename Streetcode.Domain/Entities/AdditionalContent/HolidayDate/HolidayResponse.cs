using Streetcode.Domain.Entities.AdditionalContent.HolidayDate.Dictionaries;

namespace Streetcode.Domain.Entities.AdditionalContent.HolidayDate
{
    public class HolidayResponse
    {
        public string Date { get; set; }
        public List<Holiday> Holidays { get; set; }

        public override string ToString()
        {
            var parts = Date.Split('.');
            int day = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);

            var result = $"Date: {(day < 10 ? "0" + day.ToString() : day.ToString())} {HolidayDateDictionaries.MonthesToParse[month]}\n";

            foreach (var holiday in Holidays)
            {
                result += $"Holiday: {holiday.HolidayName}\n" +
                          $"Description: {holiday.HolidayDescription}\n\n";
            }

            return result;
        }
    }
}
