using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class Follower
{
    public int FollowerId { get; set; }

    public int FollowingId { get; set; }

    [Key]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("FollowerId")]
    [InverseProperty("FollowerFollowerNavigations")]
    public virtual Member FollowerNavigation { get; set; } = null!;

    [ForeignKey("FollowingId")]
    [InverseProperty("FollowerFollowings")]
    public virtual Member Following { get; set; } = null!;
}
