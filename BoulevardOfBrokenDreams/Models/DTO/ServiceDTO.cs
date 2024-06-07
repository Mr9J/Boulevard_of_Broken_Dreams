namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public int MemberId { get; set; }
        public int? AdminId { get; set; }
        public int StatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
