using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Models;

public partial class Seller
{
    [Key]
    [Column("Seller_id")]
    public int SellerId { get; set; }

    [Column("User_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Sellers")]
    public virtual User? User { get; set; }
}
