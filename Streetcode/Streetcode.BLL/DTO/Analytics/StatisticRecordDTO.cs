﻿using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.Domain.Entities.Streetcode;

namespace Streetcode.BLL.DTO.Analytics
{
    public class StatisticRecordDto
    {
        public int Id { get; set; }
        public int QrId { get; set; }
        public int Count { get; set; }
        public string Address { get; set; }

        public int StreetcodeId { get; set; }
        public StreetcodeContent Streetcode { get; set; }
        public int StreetcodeCoordinateId { get; set; }
        public StreetcodeCoordinateDto StreetcodeCoordinate { get; set; }
    }
}
