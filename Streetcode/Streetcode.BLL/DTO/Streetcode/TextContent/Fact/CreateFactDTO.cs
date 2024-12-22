using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact
{
    public class CreateFactDTO
    {
        public string Title { get; set; }
        public string FactContent { get; set; }
        public int StreetcodeId { get; set; }
        public CreateFactImageDTO Image { get; set; }
    }
}
