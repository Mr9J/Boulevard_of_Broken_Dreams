namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class SignInSuccessDTO
    {
        public string jwt { get; set; } = string.Empty;
        public bool isAdmin { get; set; } = false;
    }
}
