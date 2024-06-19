using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class CouponDTO
    {
        public int CouponId { get; set; }

        public int ProjectId { get; set; }

        public string Code { get; set; } = null!;

        public decimal Discount { get; set; }

        public int? InitialStock { get; set; }

        public int? CurrentStock { get; set; } 

        public DateOnly? Deadline { get; set; }

        public int StatusId { get; set; }

        public string? ProjectName { get; set; }

        public string? ProjectThumbnail { get; set;}

    }
}
