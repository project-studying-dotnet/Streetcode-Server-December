using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.DTO.Analytics
{
    public class CreateStatisticRecordDTO
    {
        public int QrId { get; set; }
        public int Count { get; set; }
        public string Address { get; set; }
        public int StreetcodeId { get; set; }
        public int StreetcodeCoordinateId { get; set; }
    }
}
