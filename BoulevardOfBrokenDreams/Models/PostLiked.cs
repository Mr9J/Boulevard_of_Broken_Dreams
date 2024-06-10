using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

[Table("PostLiked")]
public partial class PostLiked
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("PostID")]
    public int PostId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("PostLikeds")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("PostId")]
    [InverseProperty("PostLikeds")]
    public virtual Post Post { get; set; } = null!;
}
