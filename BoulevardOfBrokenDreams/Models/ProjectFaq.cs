using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

[Table("ProjectFAQ")]
public partial class ProjectFaq
{
    [Key]
    [Column("ProjectFAQID")]
    public int ProjectFaqid { get; set; }

    [Column("ProjectID")]
    public int? ProjectId { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectFaqs")]
    public virtual Project? Project { get; set; }
}
