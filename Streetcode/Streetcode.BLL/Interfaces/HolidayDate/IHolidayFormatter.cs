using Streetcode.DAL.Entities.AdditionalContent.HolidayDate;

namespace Streetcode.BLL.Interfaces.HolidayFormatter
{
    public interface IHolidayFormatter
    {
        string GeneratePrompt(int day, int month, List<string> holidays);
        HolidayResponse ParseResponse(string jsonResponse);
    }
}
