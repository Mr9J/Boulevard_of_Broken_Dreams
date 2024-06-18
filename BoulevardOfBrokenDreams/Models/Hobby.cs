using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Hobby
{
    [Key]
    [Column("HobbyID")]
    public int HobbyId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("ProjectTypeID")]
    public int ProjectTypeId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Hobbies")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("ProjectTypeId")]
    [InverseProperty("Hobbies")]
    public virtual ProjectType ProjectType { get; set; } = null!;
}
