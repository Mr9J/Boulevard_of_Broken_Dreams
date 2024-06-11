﻿using System;
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

    [Column("imgUrl")]
    public string? ImgUrl { get; set; }

    [StringLength(2200)]
    public string? Location { get; set; }

    [StringLength(2200)]
    public string? Tags { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PostTime { get; set; }

    [Column("StatusID")]
    public int? StatusId { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? IsAnonymous { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Posts")]
    public virtual Member Member { get; set; } = null!;
}
