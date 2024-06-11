using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

[Table("PostComment")]
public partial class PostComment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("PostID")]
    public int PostId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [StringLength(2200)]
    public string Comment { get; set; } = null!;

    [Column("time", TypeName = "datetime")]
    public DateTime Time { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("PostComments")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("PostId")]
    [InverseProperty("PostComments")]
    public virtual Post Post { get; set; } = null!;
}
