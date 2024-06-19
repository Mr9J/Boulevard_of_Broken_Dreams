namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CreateProjectDTO
    {
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public decimal ProjectGoal { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        //public string? Thumbnail { get; set; }
        public string ProjectDetail { get; set; }
        public int ProjectTypeId {  get; set; }
        public IFormFile thumbnail { get; set; }

    }
}
