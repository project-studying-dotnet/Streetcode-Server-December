namespace Streetcode.BLL.DTO.Streetcode
{
    public class StreetcodeFilterResultDto
    {
        public int StreetcodeId { get; set; }
        required public string StreetcodeTransliterationUrl { get; set; }
        public int StreetcodeIndex { get; set; }
        public string? BlockName { get; set; }
        public string? Content { get; set; }
        public string? SourceName { get; set; }
    }
}
