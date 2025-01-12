using System.ComponentModel.DataAnnotations;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.Domain.Entities.Partners
{
    public class StreetcodePartner
    {
        [Required]
        public int StreetcodeId { get; set; }

        [Required]
        public int PartnerId { get; set; }

        public StreetcodeContent? Streetcode { get; set; }

        public Partner? Partner { get; set; }
    }
}
