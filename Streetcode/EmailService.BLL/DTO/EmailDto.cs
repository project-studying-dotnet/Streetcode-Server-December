using System.ComponentModel.DataAnnotations;

namespace EmailService.BLL.DTO
{
    public class EmailDto
    {
        [MaxLength(80)]
        public string From { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Content { get; set; }
    }
}
