namespace Streetcode.BLL.DTO.Streetcode
{
    public class GetAllStreetcodesResponseDto
    {
        public int Pages { get; set; }
        required public IEnumerable<StreetcodeDto> Streetcodes { get; set; }
    }
}