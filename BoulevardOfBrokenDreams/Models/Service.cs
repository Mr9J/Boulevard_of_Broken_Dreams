using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Service
{
    [Key]
    [Column("ServiceID")]
    public int ServiceId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("AdminID")]
    public int? AdminId { get; set; }

    [Column("StatusID")]
    public int StatusId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("Services")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Services")]
    public virtual Member Member { get; set; } = null!;

    [InverseProperty("Service")]
    public virtual ICollection<ServiceMessage> ServiceMessages { get; set; } = new List<ServiceMessage>();
}
