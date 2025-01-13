namespace Streetcode.DAL.Entities.AdditionalContent.HolidayDate.Dictionaries
{
    public static class HolidayDateDictionaries
    {
        public static Dictionary<int, string> MonthesToParse = new Dictionary<int, string>()
        {
            { 1, "sichnia" },
            { 2, "liutoho" },
            { 3, "bereznia" },
            { 4, "kvitnia" },
            { 5, "travnia" },
            { 6, "chervnia" },
            { 7, "lypnia" },
            { 8, "serpnia" },
            { 9, "veresnia" },
            { 10, "zhovtnya" },
            { 11, "lystopada" },
            { 12, "hrudnia" }
        };

        public static Dictionary<int, (int, int)> MonthesBorders = new Dictionary<int, (int, int)>()
        {
            { 1, (1, 31) },
            { 2, (1, 29) },
            { 3, (1, 30) },
            { 4, (1, 30) },
            { 5, (1, 31) },
            { 6, (1, 29) },
            { 7, (1, 31) },
            { 8, (1, 31) },
            { 9, (1, 30) },
            { 10, (1, 31) },
            { 11, (1, 30) },
            { 12, (1, 31) }
        };
    }
}
