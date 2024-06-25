namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class GroupDTO
    {
        public int? groupId { get; set; } = null;
        public string groupName { get; set; } = string.Empty;
        public SimpleUserDTO[]? users { get; set; } = null;
    }
}
