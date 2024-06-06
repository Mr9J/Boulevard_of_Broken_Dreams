namespace Mumu.Models.DTO
{
    public class ProductDataInCartDTO
    {
        public string? Thumbnail { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public decimal ProductPrice { get; set; }
        public int CurrentStock { get; set; }


        public int? Count { get; set; }



        //public DateOnly StartDate { get; set; }
        //public DateOnly EndDate { get; set; }
        //public string? ProductDescription { get; set; }
        //public int InitialStock { get; set; }
    }
}
