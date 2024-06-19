using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class CouponDetail
{
    [Key]
    [Column("CouponDetailID")]
    public int CouponDetailId { get; set; }

    [Column("CouponID")]
    public int CouponId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [ForeignKey("CouponId")]
    [InverseProperty("CouponDetails")]
    public virtual Coupon Coupon { get; set; } = null!;
}
