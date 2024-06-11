namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class VMProjectInfo
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? ProjectThumbnail { get; set; }
        public string? ProjectDescription { get; set; }
        public decimal ProjectGoal { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public int MemberId { get; set; }

        //public int StatusId { get; set; }
        //public string? GroupName { get; set; }
        //public string? MemberName { get; set; }
        //public string? MemberThumbnail { get; set; }
        //public string? StatusName { get; set; }
    }
}
