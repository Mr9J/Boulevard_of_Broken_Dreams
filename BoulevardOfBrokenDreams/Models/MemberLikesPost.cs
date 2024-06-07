using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class MemberLikesPost
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PostID")]
    public int PostId { get; set; }

    [Column("MemberID")]
    public int? MemberId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("MemberLikesPosts")]
    public virtual Member? Member { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("MemberLikesPosts")]
    public virtual Post Post { get; set; } = null!;
}
