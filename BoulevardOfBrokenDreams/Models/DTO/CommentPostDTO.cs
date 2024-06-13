namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CommentPostDTO
    {
        public int id { get; set; }
        public int postId { get; set; }
        public int userId { get; set; }
        public string comment { get; set; } = string.Empty;
        public string time { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
    }
}
