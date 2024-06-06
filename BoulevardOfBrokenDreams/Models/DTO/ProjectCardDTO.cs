namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class ProjectCardDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal ProjectGoal { get; set; }
        public int DayLeft { get; set; }
        public string? Thumbnail { get; set; }
        public decimal TotalAmount { get; set; }
        public int SponsorCount { get; set; }

    }
}
