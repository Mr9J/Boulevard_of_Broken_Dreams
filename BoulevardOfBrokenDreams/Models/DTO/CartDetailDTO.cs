namespace Mumu.Models.DTO
{
    public class CartDetailDTO
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? Thumbnail { get; set; }
        public ICollection<ProductDataInCartDTO>? Products { get; set; }

        //public List<int>? ProductId { get; set; }
        //public List<int>? Count { get; set; }
   
    }
}
