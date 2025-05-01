using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Models;

public partial class Call
{
    [Key]
    [Column("Call_id")]
    public int CallId { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [Column(TypeName = "decimal(7, 2)")]
    public decimal Costs { get; set; }

    [Column("Paid")]
    public bool Paid { get; set; }

    [Column("CallTime")]
    public DateTime CallTime { get; set; } = DateTime.Now;

    [Column("Duration")]
    public int Duration { get; set; }

    [Column("Bill_id")]
    public int? BillId { get; set; }
}
