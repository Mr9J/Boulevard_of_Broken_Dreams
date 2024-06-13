namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class VMProjectInfo
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? ProjectThumbnail { get; set; }
        public string? ProjectDescription { get; set; }
        public decimal ProjectGoal { get; set; }
        public decimal ProjectTotal { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        //public int MemberId { get; set; }

        //public int StatusId { get; set; }
        //public string? GroupName { get; set; }
        public string? MemberName { get; set; }
        public string? MemberThumbnail { get; set; }
        //public string? StatusName { get; set; }
        public bool IsLiked { get; set; }
        public List<DTOProduct> Products { get; set; } = new List<DTOProduct>();
    }
    public class DTOProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductThumbnail { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }

        public int InitialStock { get; set; }
        public int CurrentStock { get; set; }
    }
}
