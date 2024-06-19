namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class PurchasehistoryDTO
    {
        public ICollection<OrderDTO> orderDTOs { get; set; }
        public ICollection<ProjectCardDTO> projectCardDTOs { get; set; }
    }
}
