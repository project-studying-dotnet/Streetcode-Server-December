using System.ComponentModel.DataAnnotations;

namespace EmailService.BLL.DTO
{
    public class EmailDto
    {
        public required IEnumerable<string> ToEmail { get; set; }

        [MaxLength(80)]
        public required string FromText { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public required string Content { get; set; }
    }
}
