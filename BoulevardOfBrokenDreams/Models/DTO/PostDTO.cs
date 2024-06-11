namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class PostDTO
    {
        public string postId { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string userImg { get; set; } = string.Empty;
        public string caption { get; set; } = string.Empty;
        public string imgUrl { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string tags { get; set; } = string.Empty;
        public string postTime { get; set; } = string.Empty;
        public string isAnonymous { get; set; } = string.Empty;
    }
}
