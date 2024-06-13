namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class ServiceMessageDTO
    {
        public int MessageId { get; set; }
        public int ServiceId { get; set; }
        public int? MemberId { get; set; }
        public int? AdminId { get; set; }
        public string? MessageContent { get; set; }
        public DateTime MessageDate { get; set; }
        public bool IsRead { get; set; }
    }
}
