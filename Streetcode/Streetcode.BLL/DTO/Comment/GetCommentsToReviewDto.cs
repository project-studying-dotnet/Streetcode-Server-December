namespace Streetcode.BLL.DTO.Comment
{
    public class GetCommentsToReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DateModified { get; set; }
        public string Content { get; set; }
        public int StreetcodeId { get; set; }
    }
}
