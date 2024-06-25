namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class UpdatePostDTO
    {
        public string caption { get; set; } = string.Empty;
        public string file { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string tags { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public int postId { get; set; }
        public string isAlert { get; set; } = string.Empty;
    }
}
