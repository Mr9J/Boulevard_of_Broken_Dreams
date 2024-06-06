using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Post
{
    [Key]
    [Column("PostID")]
    public int PostId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [StringLength(2200)]
    public string? Caption { get; set; }

    public string? Tags { get; set; }

    [Column("LikesPostID")]
    public int? LikesPostId { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public string? ImageUrl { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Posts")]
    public virtual Member Member { get; set; } = null!;

    [InverseProperty("Post")]
    public virtual ICollection<MemberLikesPost> MemberLikesPosts { get; set; } = new List<MemberLikesPost>();
}
