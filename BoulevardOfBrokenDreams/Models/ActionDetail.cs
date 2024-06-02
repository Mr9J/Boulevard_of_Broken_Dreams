using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class ActionDetail
{
    [Key]
    [Column("ActionDetailID")]
    public int ActionDetailId { get; set; }

    [Column("ActionID")]
    public int ActionId { get; set; }

    [Column("ActionTypeID")]
    public int ActionTypeId { get; set; }

    [ForeignKey("ActionId")]
    [InverseProperty("ActionDetails")]
    public virtual Action Action { get; set; } = null!;

    [ForeignKey("ActionTypeId")]
    [InverseProperty("ActionDetails")]
    public virtual ActionType ActionType { get; set; } = null!;
}
