namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class MemberInfoDTO
    {
        public int id { get; set; }
        public string nickname { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string avatar { get; set; } = string.Empty;
        public DateTime time { get; set; }
        public int postCount { get; set; }
        public int followCount { get; set; }
        public int memberStatus { get; set; }
        public string banner { get; set; } = string.Empty;
        public string authenticationProvider { get; set; } = string.Empty;
        public string showContactInfo { get; set; } = string.Empty;
        public GetProjectDTO[] projects { get; set; } = new GetProjectDTO[0];
    }
}
