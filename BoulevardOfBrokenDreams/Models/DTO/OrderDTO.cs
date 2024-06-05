namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public int MemberId { get; set; }
        public DateTime ShipDate { get; set; }
        public int ShipmentStatusId { get; set; }
        public int PaymentMethodId { get; set; }
        public int PaymentStatusId { get; set; }
        public decimal? Donate { get; set; }
        public MemberDTO Member { get; set; }
        public OrderDetailDTO OrderDetails { get; set; }
        public int ProjectId { get; set; }
    }
}
