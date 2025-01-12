using System.ComponentModel.DataAnnotations;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.Domain.Entities.Toponyms
{
    public class StreetcodeToponym
    {
        [Required]
        public int StreetcodeId { get; set; }

        [Required]
        public int ToponymId { get; set; }

        public StreetcodeContent? Streetcode { get; set; }

        public Toponym? Toponym { get; set; }
    }
}
