using System.ComponentModel.DataAnnotations;

namespace EmailService.BLL.DTO
{
    public class EmailDto
    {
        public IEnumerable<string> ToEmail { get; set; }

        [MaxLength(80)]
        public string FromText { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Content { get; set; }
    }
}
