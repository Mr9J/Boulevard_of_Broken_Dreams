namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class GetProjectDTO
    {
        public int projectId { get; set; }
        public string projectName { get; set; } = string.Empty;
        public string projectDescription { get; set; } = string.Empty;
        public decimal projectGoal { get; set; }
        public DateOnly projectStartDate { get; set; }
        public DateOnly projectEndDate { get; set; }
        public int projectGroupId { get; set; }
        public string projectThumbnail { get; set; } = string.Empty;
        public int projectStatusId { get; set; }
    }
}
