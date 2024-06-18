using System.Collections.Generic;

namespace BoulevardOfBrokenDreams.Models.DTO

{
    public class CreateOrderDTO
    {
       public int MemberId { get; set; }
       public int PaymentMethodId { get; set; }
        public int ProjectId { get; set; }

        public List<int>? ProductId { get; set; }

        public decimal? Donate { get; set; }

        public List<OrderProductDTO>? ProductData { get; set; }

        public int Discount { get; set; }



    }

    public class OrderProductDTO
    {
        public string? ProductId { get; set; }
        public int Count { get; set; }
    }
}
