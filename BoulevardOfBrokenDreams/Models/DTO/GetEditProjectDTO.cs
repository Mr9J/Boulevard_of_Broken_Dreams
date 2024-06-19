namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class GetEditProjectDTO
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public decimal ProjectGoal { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set;}
        public string ProjectDetails { get; set; }
        public string thumbnail { get; set; }
        public int ProjectTypeId { get; set; }
    }
}
