namespace BoulevardOfBrokenDreams.Models.DTO;
    public class ProjectCardDTO
    {
        public int ProjectId { get; set; }
        public int MemberId { get; set; }
        public decimal ProjectGoal { get; set; }

        public decimal Total {  get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string?  Thumbnail { get; set; }
        public MemberDTO? Member { get; set; }
        public ICollection<ProductCardDTO>? Products { get; set; }

        public List<int>? ProductInCart { get; set; }

        public List<int>? ProductInCartCount { get; set; }
    }