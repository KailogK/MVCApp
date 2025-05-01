using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Models;

public partial class LabDBContext : DbContext
{
    public LabDBContext()
    {
    }

    public LabDBContext(DbContextOptions<LabDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Call> Calls { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Programme> Programs { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-50MRGDS;Database=TelecomDB;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__4A311D2FB30E6136");

            entity.HasOne(d => d.User).WithMany(p => p.Admins).HasConstraintName("FK__Admin__User_id__6E01572D");
        });

        modelBuilder.Entity<Bill>()
            .HasOne(b => b.Phone)  // Bill has one Phone
            .WithMany(p => p.Bills) // Phone has many Bills
            .HasForeignKey(b => b.PhoneNumber) // Foreign key is PhoneNumber
            .OnDelete(DeleteBehavior.Restrict); // Configure delete behavior if needed

        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PK__Calls__19E7F093973A4759");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsUnicode(false);
            entity.Property(e => e.Costs).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.Paid).IsRequired();
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__75A2D320B56299E0");

            entity.HasOne(d => d.User).WithMany(p => p.Clients).HasConstraintName("FK__Clients__User_id__71D1E811");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.PhoneNumber).HasName("PK__Phones__85FB4E392FAA14E1");
        });

        modelBuilder.Entity<Programme>(entity =>
        {
            entity.HasKey(e => e.ProgramName).HasName("PK__Programs__4F925711C949785D");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__Sellers__016148B16AE97788");

            entity.HasOne(d => d.User).WithMany(p => p.Sellers).HasConstraintName("FK__Sellers__User_id__6B24EA82");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__206A9DF8876E0A22");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
