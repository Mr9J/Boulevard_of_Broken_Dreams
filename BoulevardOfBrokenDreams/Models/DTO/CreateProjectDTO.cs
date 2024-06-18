namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CreateProjectDTO
    {
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string ProjectGoal { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        //public string? Thumbnail { get; set; }
        public string ProjectDetail { get; set; }
        public string ProjectTypeId {  get; set; }

    }
}
