using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Media.Video
{
    public class VideoCreateDTO
    {
        [Required]
        public string Url { get; set; }

        [Required]
        public int StreetcodeId { get; set; }
    }
}
