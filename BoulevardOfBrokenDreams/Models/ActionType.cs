using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class ActionType
{
    [Key]
    [Column("ActionTypeID")]
    public int ActionTypeId { get; set; }

    [StringLength(50)]
    public string ActionName { get; set; } = null!;

    [InverseProperty("ActionType")]
    public virtual ICollection<ActionDetail> ActionDetails { get; set; } = new List<ActionDetail>();
}
