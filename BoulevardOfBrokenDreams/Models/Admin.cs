using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Admin
{
    [Key]
    [Column("AdminID")]
    public int AdminId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Admins")]
    public virtual Member Member { get; set; } = null!;

    [InverseProperty("Admin")]
    public virtual ICollection<ServiceMessage> ServiceMessages { get; set; } = new List<ServiceMessage>();

    [InverseProperty("Admin")]
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
