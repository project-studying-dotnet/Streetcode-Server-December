using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
