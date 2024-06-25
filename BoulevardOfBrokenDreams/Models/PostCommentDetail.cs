using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

[Table("PostCommentDetail")]
public partial class PostCommentDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? LikesStatus { get; set; }

    [Column("PostCommentID")]
    public int PostCommentId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("PostCommentDetails")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("PostCommentId")]
    [InverseProperty("PostCommentDetails")]
    public virtual PostComment PostComment { get; set; } = null!;
}
