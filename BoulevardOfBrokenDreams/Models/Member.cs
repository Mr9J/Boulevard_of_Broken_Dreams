using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Member
{
    [Key]
    [Column("MemberID")]
    public int MemberId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? Password { get; set; }

    [StringLength(50)]
    public string? Nickname { get; set; }

    public string? Thumbnail { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public string? MemberIntroduction { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RegistrationTime { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    [InverseProperty("Member")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("Member")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("Member")]
    public virtual ICollection<GroupDetail> GroupDetails { get; set; } = new List<GroupDetail>();

    [InverseProperty("Member")]
    public virtual ICollection<LikeDetail> LikeDetails { get; set; } = new List<LikeDetail>();

    [InverseProperty("Member")]
    public virtual ICollection<MemberInterestProjectType> MemberInterestProjectTypes { get; set; } = new List<MemberInterestProjectType>();

    [InverseProperty("Member")]
    public virtual ICollection<MemberLikesPost> MemberLikesPosts { get; set; } = new List<MemberLikesPost>();

    [InverseProperty("Member")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Member")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("Memeber")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("Member")]
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
