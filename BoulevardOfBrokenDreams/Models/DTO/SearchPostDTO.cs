namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class SearchPostDTO
    {
        public string keyword { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;

        public int page { get; set; } = 0;

    }
}
