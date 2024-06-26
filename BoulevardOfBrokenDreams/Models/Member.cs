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

    [Column("EID")]
    public Guid? Eid { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Verified { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? ResetPassword { get; set; }

    [Column("StatusID")]
    public int? StatusId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Banner { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? AuthenticationProvider { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? ShowContactInfo { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<Action> Actions { get; set; } = new List<Action>();

    [InverseProperty("Member")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    [InverseProperty("Member")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("Member")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("FollowerNavigation")]
    public virtual ICollection<Follower> FollowerFollowerNavigations { get; set; } = new List<Follower>();

    [InverseProperty("Following")]
    public virtual ICollection<Follower> FollowerFollowings { get; set; } = new List<Follower>();

    [InverseProperty("Member")]
    public virtual ICollection<GroupDetail> GroupDetails { get; set; } = new List<GroupDetail>();

    [InverseProperty("Member")]
    public virtual ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();

    [InverseProperty("Member")]
    public virtual ICollection<LikeDetail> LikeDetails { get; set; } = new List<LikeDetail>();

    [InverseProperty("Member")]
    public virtual ICollection<MemberInterestProjectType> MemberInterestProjectTypes { get; set; } = new List<MemberInterestProjectType>();

    [InverseProperty("Member")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Member")]
    public virtual ICollection<PostCommentDetail> PostCommentDetails { get; set; } = new List<PostCommentDetail>();

    [InverseProperty("Member")]
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    [InverseProperty("Member")]
    public virtual ICollection<PostLiked> PostLikeds { get; set; } = new List<PostLiked>();

    [InverseProperty("Member")]
    public virtual ICollection<PostSaved> PostSaveds { get; set; } = new List<PostSaved>();

    [InverseProperty("Member")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("Member")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("Member")]
    public virtual ICollection<ServiceMessage> ServiceMessages { get; set; } = new List<ServiceMessage>();

    [InverseProperty("Member")]
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
