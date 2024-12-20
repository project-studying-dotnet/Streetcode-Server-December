using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact
{
    public class FactReorderDto
    {
        [Required]
        public int StreetcodeId { get; set; }
        [Required]
        public List<int> IdPositions { get; set; } = null!;
    }
}
