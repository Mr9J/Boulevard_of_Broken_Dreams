namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CurrentUserDTO
    {
        public int id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string jwt { get; set; } = string.Empty;
    }
}
