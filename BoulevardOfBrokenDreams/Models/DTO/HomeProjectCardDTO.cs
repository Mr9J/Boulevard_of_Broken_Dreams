namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class HomeProjectCardDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal ProjectGoal { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DayLeft { get; set; }
        public string? Thumbnail { get; set; }
        public decimal TotalAmount { get; set; }
        public int SponsorCount { get; set; }
        public int clicked {  get; set; }

    }
}