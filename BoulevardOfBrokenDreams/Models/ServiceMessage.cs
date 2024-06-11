using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class ServiceMessage
{
    [Key]
    [Column("MessageID")]
    public int MessageId { get; set; }

    [Column("ServiceID")]
    public int ServiceId { get; set; }

    [Column("MemberID")]
    public int? MemberId { get; set; }

    [Column("AdminID")]
    public int? AdminId { get; set; }

    public string MessageContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime MessageDate { get; set; }

    public bool IsRead { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("ServiceMessages")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("ServiceMessages")]
    public virtual Member? Member { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("ServiceMessages")]
    public virtual Service Service { get; set; } = null!;
}
