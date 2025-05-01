using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Models;

[Index("PhoneNumber", Name = "UQ__Clients__85FB4E385F5B0B7A", IsUnique = true)]
public partial class Client
{
    [Key]
    [Column("Client_id")]
    public int ClientId { get; set; }

    [Column("AFM")]
    [StringLength(50)]
    [Unicode(false)]
    public string Afm { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [Column("User_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Clients")]
    public virtual User? User { get; set; }
}
