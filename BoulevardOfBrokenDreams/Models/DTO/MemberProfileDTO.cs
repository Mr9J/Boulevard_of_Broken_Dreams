namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class MemberProfileDTO
    {
        public int id { get; set; } = 0;
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string memberIntroduction { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string verified { get; set; } = "N";
        public int status { get; set; } = 7;

        public string banner { get; set; } = string.Empty;
        public string authenticationProvider { get; set; } = string.Empty;
        public string showContactInfo { get; set; } = string.Empty;
    }
}
