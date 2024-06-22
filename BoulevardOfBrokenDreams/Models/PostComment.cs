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
    [Column("PostCommentID")]
    public int PostCommentId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("PostID")]
    public int PostId { get; set; }

    [StringLength(2200)]
    public string Comment { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Time { get; set; }

    [Column("ParentCommentID")]
    public int? ParentCommentId { get; set; }

    [InverseProperty("ParentComment")]
    public virtual ICollection<PostComment> InverseParentComment { get; set; } = new List<PostComment>();

    [ForeignKey("MemberId")]
    [InverseProperty("PostComments")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("ParentCommentId")]
    [InverseProperty("InverseParentComment")]
    public virtual PostComment? ParentComment { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("PostComments")]
    public virtual Post Post { get; set; } = null!;

    [InverseProperty("PostComment")]
    public virtual ICollection<PostCommentDetail> PostCommentDetails { get; set; } = new List<PostCommentDetail>();
}
