namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CommentPostDTO
    {
        public int postCommentID { get; set; }

        public int memberID { get; set; }
        public string nickname { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
        public int postID { get; set; }
        public string comment { get; set; } = string.Empty;
        public DateTime date { get; set; }
        public int? parentCommentID { get; set; } = null;
        public CommentPostDTO[]? childComments { get; set; } = null;
        public PostCommentDetailDTO? postCommentDetail { get; set; } = null;
    }
}
