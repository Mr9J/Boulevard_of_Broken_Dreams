namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CustomerMessageDTO
    {
        public int ServiceId { get; set; }
        public int? MemberId { get; set; }
        public int? AdminId { get; set; }
        public string? MessageContent { get; set; }
        public DateTime MessageDate { get; set; }
        public int MessageCount { get; set; }  // 新增的消息數量屬性
        public int UnreadMessages { get; set; }
    }
}
