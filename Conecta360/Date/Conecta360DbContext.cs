using Microsoft.EntityFrameworkCore;
using Conect360.Models;

namespace Conect360.Data
{
    public class Conecta360DbContext : DbContext
    {
        public Conecta360DbContext(DbContextOptions<Conecta360DbContext> options)
            : base(options)
        {
        }

        // Tabla principal
        public DbSet<Contacto> Contactos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contacto>(entity =>
            {
                entity.ToTable("Contactos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Apellido)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Telefono)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.Email)
                      .HasMaxLength(150);

                entity.Property(e => e.Categoria)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasDefaultValue("General");

                entity.Property(e => e.Notas)
                      .HasMaxLength(500);

                entity.Property(e => e.FechaRegistro)
                      .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
