using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Models;

public partial class Phone
{
    [Column("PhoneNumber")]
    [Key]
    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [Column("ProgramName")]
    [StringLength(50)]
    [Unicode(false)]
    public string ProgramName { get; set; } = null!;

    [Column("ProgramPaid")]
    public bool ProgramPaid { get; set; }
    [Column("ProgramEnds")]
    public DateTime ProgramEnds { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
