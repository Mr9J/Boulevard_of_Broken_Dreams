using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Comment
{
    [Key]
    [Column("CommentID")]
    public int CommentId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("ProjectID")]
    public int ProjectId { get; set; }

    public string CommentMsg { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public int? Liked { get; set; }

    [Column("ParentID")]
    public int? ParentId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Comments")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("Comments")]
    public virtual Project Project { get; set; } = null!;
}
