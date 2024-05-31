using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Action
{
    [Key]
    [Column("ActionID")]
    public int ActionId { get; set; }

    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [InverseProperty("Action")]
    public virtual ICollection<ActionDetail> ActionDetails { get; set; } = new List<ActionDetail>();

    [ForeignKey("MemberId")]
    [InverseProperty("Actions")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("Actions")]
    public virtual Project Project { get; set; } = null!;
}
