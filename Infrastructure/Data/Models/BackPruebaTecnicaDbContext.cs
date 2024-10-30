using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BackPruebaTecnica.Infrastructure.Data.Models;

public partial class BackPruebaTecnicaDbContext : DbContext
{
    public BackPruebaTecnicaDbContext()
    {
    }

    public BackPruebaTecnicaDbContext(DbContextOptions<BackPruebaTecnicaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HistorialPrestamo> HistorialPrestamos { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KEITOR\\MSSQLSERVER01;Database=PruebaTecnicaDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistorialPrestamo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC07DF50F5F4");

            entity.Property(e => e.EstadoAnterior).HasMaxLength(20);
            entity.Property(e => e.EstadoNuevo).HasMaxLength(20);
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Prestamo).WithMany(p => p.HistorialPrestamos)
                .HasForeignKey(d => d.PrestamoId)
                .HasConstraintName("FK__Historial__Prest__534D60F1");
        });


        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prestamo__3214EC070981E2BD");

            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Prestamos__Usuar__4F7CD00D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0719A8160D");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC0787B978D2");

            entity.HasIndex(e => e.Usuario, "UQ__Usuarios__E3237CF7E048553F").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Pass)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RolId).HasDefaultValue(2);
            entity.Property(e => e.Usuario)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Usuario");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
