using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Coupon
{
    [Key]
    [Column("CouponID")]
    public int CouponId { get; set; }

    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal Discount { get; set; }

    public int InitialStock { get; set; }

    public int CurrentStock { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateTime { get; set; }

    [InverseProperty("Coupon")]
    public virtual ICollection<CouponDetail> CouponDetails { get; set; } = new List<CouponDetail>();

    [ForeignKey("ProjectId")]
    [InverseProperty("Coupons")]
    public virtual Project Project { get; set; } = null!;
}
