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
