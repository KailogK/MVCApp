using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Models;

public partial class Bill
{
    [Key]
    [Column("Bill_id")]
    public int BillId { get; set; }

    [Column("PhoneNumber")]
    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [ForeignKey("PhoneNumber")] // Explicit foreign key
    public virtual Phone Phone { get; set; }

    [Column("BillPaid")]
    public bool BillPaid { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Costs { get; set; }
}
