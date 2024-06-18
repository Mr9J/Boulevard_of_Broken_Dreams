using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Project
{
    [Key]
    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [StringLength(100)]
    public string ProjectName { get; set; } = null!;

    public string? ProjectDescription { get; set; }

    [Column(TypeName = "money")]
    public decimal ProjectGoal { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    public string? Campaign { get; set; }

    public string? Updates { get; set; }

    [StringLength(100)]
    public string? Thumbnail { get; set; }

    [Column("StatusID")]
    public int StatusId { get; set; }

    public string? ProjectDetails { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<Action> Actions { get; set; } = new List<Action>();

    [InverseProperty("Project")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("Project")]
    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    [ForeignKey("GroupId")]
    [InverseProperty("Projects")]
    public virtual Group? Group { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    [ForeignKey("MemberId")]
    [InverseProperty("Projects")]
    public virtual Member Member { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Project")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectFaq> ProjectFaqs { get; set; } = new List<ProjectFaq>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectIdtype> ProjectIdtypes { get; set; } = new List<ProjectIdtype>();

    [ForeignKey("StatusId")]
    [InverseProperty("Projects")]
    public virtual Status Status { get; set; } = null!;
}
