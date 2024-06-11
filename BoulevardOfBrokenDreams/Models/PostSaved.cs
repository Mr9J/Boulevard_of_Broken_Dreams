using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

[Table("PostSaved")]
public partial class PostSaved
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("PostID")]
    public int PostId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("PostSaveds")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("PostId")]
    [InverseProperty("PostSaveds")]
    public virtual Post Post { get; set; } = null!;
}
