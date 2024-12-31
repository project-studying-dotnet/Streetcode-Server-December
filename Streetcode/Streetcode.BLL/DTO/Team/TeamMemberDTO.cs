namespace Streetcode.BLL.DTO.Team
{
    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public int ImageId { get; set; }
        public List<TeamMemberLinkDto> TeamMemberLinks { get; set; } = new List<TeamMemberLinkDto>();
        public List<PositionDto> Positions { get; set; } = new List<PositionDto>();
    }
}
