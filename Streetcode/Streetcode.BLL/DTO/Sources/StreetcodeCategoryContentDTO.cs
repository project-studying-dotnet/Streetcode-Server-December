using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Sources
{
    public class StreetcodeCategoryContentDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }

        [Required]
        public int SourceLinkCategoryId { get; set; }

        [Required]
        public int StreetcodeId { get; set; }
	}
}
