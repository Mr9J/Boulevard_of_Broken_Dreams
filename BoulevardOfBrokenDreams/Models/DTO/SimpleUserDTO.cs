namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class SimpleUserDTO
    {
        public int memberId { get; set; }
        public string username { get; set; } = string.Empty;

        public string nickname { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
        public int authStatus { get; set; }
    }
}
