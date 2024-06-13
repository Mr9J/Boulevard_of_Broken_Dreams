namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class MemberDTO
    {
        public int MemberId { get; set; }

        public string Username { get; set; } = null!;

        public string? Nickname { get; set; }

        public string? Thumbnail { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? MemberIntroduction { get; set; }

        public string? Phone { get; set; }

        public int? StatusId { get; set; }

        public DateTime? RegistrationTime { get; set; }

        public GroupDetailDTO GroupDetail { get; set; }
    }
}
