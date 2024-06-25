namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class GroupUpdateDTO
    {
        public int groupId { get; set; }
        public string groupName { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public int projectId { get; set; }
        public string action { get; set; } = string.Empty;
    }
}
